#include "Hub.h"
#include "ui_Hub.h"

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
