#ifndef HUB_H
#define HUB_H

#include <QMainWindow>

QT_BEGIN_NAMESPACE
namespace Ui {
class Hub;
}
QT_END_NAMESPACE

class Hub : public QMainWindow
{
    Q_OBJECT

public:
    Hub(QWidget *parent = nullptr);
    ~Hub();

    void CloseActiveSim();

private:
    Ui::Hub *ui;

    QWidget *mActiveSim = nullptr;

private slots:
    void on_pb2dEFCmd_clicked();
};
#endif // HUB_H
