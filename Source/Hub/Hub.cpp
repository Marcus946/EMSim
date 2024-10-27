#include "Hub.h"
#include "ui_Hub.h"
#include "ElectricField2DControl.h"

#include "SharedMemoryManager.h"

Hub::Hub(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::Hub)
{
    ui->setupUi(this);
}

Hub::~Hub()
{
    delete ui;
}

void Hub::CloseActiveSim()
{
    delete mActiveSim;
    mActiveSim = nullptr;
}

void Hub::on_pb2dEFCmd_clicked()
{
    ElectricField2DControl* ef2DControl = new ElectricField2DControl(this);
    ef2DControl->setAttribute(Qt::WA_DeleteOnClose);
    mActiveSim = ef2DControl;
    ef2DControl->show();
}
