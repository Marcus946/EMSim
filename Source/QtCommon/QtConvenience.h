#ifndef QTCONVENIENCE_H
#define QTCONVENIENCE_H

#include <QtWidgets/QTableWidget>

namespace TableWidget
{
	void RemoveRows(
		QTableWidget *table,
		const QList<QModelIndex> &indices);

	QList<QModelIndex> SelectedRows(
		const QTableWidget *table);
}

#endif // !QTCONVENIENCE_H
