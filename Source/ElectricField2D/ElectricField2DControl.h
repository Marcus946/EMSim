#ifndef EF2DCONTROL_H
#define EF2DCONTROL_H

#include "Hub.h"
#include "SharedMemoryManager.h"
#include "PointCharge.h"

#include <set>
#include <qmetatype.h>
#include <QMainWindow>

QT_BEGIN_NAMESPACE
namespace Ui{
    class EF2DControl;
}
QT_END_NAMESPACE

class ElectricField2DControl : public QMainWindow
{
    Q_OBJECT

public:
    ElectricField2DControl(
        QWidget *parent = nullptr);
    ~ElectricField2DControl();

    struct ControlStruct
    {
    };

private:
    void closeEvent(
        QCloseEvent *event);

    bool UpdateSharedMemory();

    bool GetSharedMemory();

    void AddChargeToTable(
        const PointCharge &pointCharge);

    void UpdateRow(
        const PointCharge &newCharge,
        const int row);

    void Tick();

    const float TicksPerSecond = 20.0f;

    Ui::EF2DControl *ui;
    Hub *mParent = nullptr;

    ControlStruct mControlStruct;

private slots:
    void on_pbAddCharge_clicked();
    void on_pbRemoveSelected_clicked();
    void on_tbCharges_itemSelectionChanged();
};

#endif // !EF2DCONTROL_H