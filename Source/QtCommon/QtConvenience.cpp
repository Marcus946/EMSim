#include "QtConvenience.h"

#include <vector>

void TableWidget::RemoveRows(
        QTableWidget *table,
        const QList<QModelIndex> &indices)
{
    std::vector<int> deletedRows;

    // must delete backwards so that table indices don't change
    QList<QModelIndex>::const_reverse_iterator it;
    for (it = indices.rbegin(); it != indices.rend(); ++it) {
        if (std::count(deletedRows.begin(), deletedRows.end(), it->row()) == 0) {
            table->removeRow(it->row());
            deletedRows.push_back(it->row());
        }
    }
}

QList<QModelIndex> TableWidget::SelectedRows(
        const QTableWidget *table)
{
    std::vector<int> rows;

    QItemSelectionModel *selectionModel = table->selectionModel();
    QList<QModelIndex> selectedRows = selectionModel->selectedRows();

    return selectedRows;
}
