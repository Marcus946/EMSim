#include "ElectricField2DControl.h"
#include "ui_ElectricField2DControl.h"
#include "QtConvenience.h"
#include "qtimer.h"

ElectricField2DControl::ElectricField2DControl(QWidget *parent)
    : QMainWindow(parent),
    mParent((Hub *)parent),
    ui(new Ui::EF2DControl)
{
    // the order of a lot of these function calls matter
    SharedMemoryManager::Open();

    ui->setupUi(this);

    on_tbCharges_itemSelectionChanged();

    setWindowModality(Qt::WindowModal);

    QTimer *timer = new QTimer(this);
    connect(timer, &QTimer::timeout, this, &ElectricField2DControl::Tick);
    timer->start(1000.0f / TicksPerSecond);

    //// mph used for testing
    //on_pbAddCharge_clicked();
    //ui->sbPositionX->setValue(-2.0f);
    //ui->sbPositionY->setValue(-2.0f);
    //on_pbAddCharge_clicked();
    //ui->sbPositionX->setValue(2.5f);
    //ui->sbPositionY->setValue(2.5f);
    //on_pbAddCharge_clicked();
}

ElectricField2DControl::~ElectricField2DControl(){
    delete ui;
}

void ElectricField2DControl::closeEvent(
        QCloseEvent *event)
{
    mParent->CloseActiveSim();
}

/*
* layout of shared memory:
* sizeof(int) - number of charges
* sizeof(PointCharge) * number of charges - charges data
* sizeof(ControlStruct) - control struct
*/
bool ElectricField2DControl::UpdateSharedMemory()
{
    const int rowCount = ui->tbCharges->rowCount();

    const int totalByteCount = sizeof(rowCount) + rowCount * sizeof(PointCharge) + sizeof(ControlStruct);
    std::unique_ptr<unsigned char> bytes(new unsigned char[totalByteCount]);
    int bytesWritten = 0;

    bool ok = true;

    ok = ok && 0 == memcpy_s(bytes.get(), sizeof(rowCount), &rowCount, sizeof(rowCount));
    bytesWritten += sizeof(rowCount);

    for (int i = 0; i < rowCount; i++) {
        QVariant item = ui->tbCharges->item(i, 0)->data(Qt::UserRole);
        PointCharge charge = item.value<PointCharge>();

        ok = ok && charge.Serialize(bytes.get() + bytesWritten);
        bytesWritten += sizeof(PointCharge);
    }

    ok = ok && 0 == memcpy_s(bytes.get() + bytesWritten, sizeof(ControlStruct), &mControlStruct, sizeof(ControlStruct));
    // bytesWritten += sizeof(rowCount);

    ok = ok && 0 == SharedMemoryManager::Write(bytes.get(), totalByteCount);

    return ok;
}

bool ElectricField2DControl::GetSharedMemory()
{
    int byteCount, bytesRead = 0;
    constexpr int bufSize = SharedMemoryManager::BufferSize();
    char buf[bufSize];
    bool success = SharedMemoryManager::Read(buf, bufSize);

    int chargeCount;

    if (success) {
        chargeCount = *(int *)buf;
        bytesRead += sizeof(int);

        int currentChargeCount = ui->tbCharges->rowCount();
        int deltaChargeCount = chargeCount - currentChargeCount;

        if (deltaChargeCount > 0) {
            PointCharge temp;
            AddChargeToTable(temp);
        }
        else if (deltaChargeCount < 0) {
            for (int i = 0; i < -deltaChargeCount; i++) {
                ui->tbCharges->removeRow(0);
            }
        }

        for (int i = 0; i < chargeCount; i++) {
            PointCharge charge = *(PointCharge *)(buf + bytesRead);
            bytesRead += sizeof(PointCharge);
            UpdateRow(charge, i);
        }

        mControlStruct = *(ControlStruct *)(buf + bytesRead);
    }

    return success;
}

void ElectricField2DControl::AddChargeToTable(
        const PointCharge &pointCharge)
{
    const int newRow = ui->tbCharges->rowCount();

    ui->tbCharges->insertRow(newRow);

    for (int column = 0; column < 3; column++) {
        QTableWidgetItem *item = new QTableWidgetItem;
        ui->tbCharges->setItem(newRow, column, item);
    }

    UpdateRow(pointCharge, newRow);

    ui->tbCharges->setCurrentCell(newRow, 0);
}

// mph maybe use each member of pointcharge as value of that column instead of struct as column 0 value for this whole class
void ElectricField2DControl::UpdateRow(
        const PointCharge &newCharge,
        const int row)
{
    QVariant newData;
    newData.setValue(newCharge);
    ui->tbCharges->item(row, 0)->setData(Qt::UserRole, newData);

    // create items
    QString chargeStr, xPosStr, yPosStr;
    QTableWidgetItem *item = ui->tbCharges->item(row, 0);
    ui->tbCharges->item(row, 0)->setText(chargeStr.setNum(newCharge.mChargeNC, 'd', 3));
    ui->tbCharges->item(row, 1)->setText(xPosStr.setNum(newCharge.mPosition.x, 'd', 3));
    ui->tbCharges->item(row, 2)->setText(yPosStr.setNum(newCharge.mPosition.y, 'd', 3));

    UpdateSharedMemory();
}

void ElectricField2DControl::Tick()
{
    int byteCount;
    constexpr int bufSize = SharedMemoryManager::BufferSize();
    char buf[bufSize];
    bool success = SharedMemoryManager::Read(buf, bufSize);

    // skip point charge count because simulation can't change this.
    // Simulation side can only make "soft changes" like moving a point, as opposed to removing a charge.
    int bytesRead = sizeof(int);

    for (int i = 0; i < ui->tbCharges->rowCount(); i++) {
        PointCharge charge = *(PointCharge *)(buf + bytesRead);
        bytesRead += sizeof(PointCharge);
        
        UpdateRow(charge, i);
    }

    UpdateSharedMemory();
}

void ElectricField2DControl::on_pbAddCharge_clicked()
{
    float charge = ui->sbCharge->value();
    Vec2 pos(ui->sbPositionX->value(), ui->sbPositionY->value());
    PointCharge pointCharge(charge, pos);

    AddChargeToTable(pointCharge);
}

void ElectricField2DControl::on_pbRemoveSelected_clicked()
{
    QItemSelectionModel *selectionModel = ui->tbCharges->selectionModel();
    QList<QModelIndex> selectedRows = selectionModel->selectedIndexes();

    TableWidget::RemoveRows(ui->tbCharges, selectedRows);

    UpdateSharedMemory();

    ui->tbCharges->setCurrentCell(ui->tbCharges->rowCount() - 1, 0);
    on_tbCharges_itemSelectionChanged();
}

void ElectricField2DControl::on_tbCharges_itemSelectionChanged()
{
    int rowCount = ui->tbCharges->rowCount();
    int selectedRowCount = TableWidget::SelectedRows(ui->tbCharges).size();

    char labelText[100];
    snprintf(labelText, sizeof(labelText), "Remove Selected (%d/%d)", selectedRowCount, rowCount);
    ui->pbRemoveSelected->setText(QString(labelText));
}
