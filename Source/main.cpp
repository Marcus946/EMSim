#include "Hub.h"
#include "PointCharge.h"

#include <QApplication>

int main(int argc, char *argv[])
{
    QApplication a(argc, argv);

    qRegisterMetaType<PointCharge>();

    Hub w;
    w.show();
    return a.exec();
}
