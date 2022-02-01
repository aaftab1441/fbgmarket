(function () {
    var selectedIds; function onGridViewInit(s, e) { AddAdjustmentDelegate(adjustGridView); updateToolbarButtonsState() }
    function onGridViewSelectionChanged(s, e) { updateToolbarButtonsState() }
    function adjustGridView() { productGridView.AdjustControl() }
    function updateToolbarButtonsState() { var enabled = productGridView.GetSelectedRowCount() > 0; pageToolbar.GetItemByName("Delete").SetEnabled(enabled); pageToolbar.GetItemByName("Edit").SetEnabled(productGridView.GetFocusedRowIndex() !== -1) }
    function onPageToolbarItemClick(s, e) { switch (e.item.name) { case "ToggleFilterPanel": toggleFilterPanel(); break; break; case "Edit": productGridView.StartEditRow(productGridView.GetFocusedRowIndex()); break; case "Delete": deleteSelectedRecords(); break } }
    function toggleFilterPanel() { filterPanel.Toggle() }
    function onFilterPanelExpanded(s, e) { adjustPageControls(); searchButtonEdit.SetFocus() }
    function onGridViewBeginCallback(s, e) { e.customArgs.SelectedRows = selectedIds }
    function getSelectedFieldValuesCallback(values) { selectedIds = values.join(','); gridView.PerformCallback({ customAction: 'delete' }) }
    window.onGridViewBeginCallback = onGridViewBeginCallback; window.onGridViewInit = onGridViewInit; window.onGridViewSelectionChanged = onGridViewSelectionChanged; window.onPageToolbarItemClick = onPageToolbarItemClick; window.onFilterPanelExpanded = onFilterPanelExpanded
})()