(function () {
    function onSchedulerToolbarItemClick(s, e) { switch (e.item.name) { case 'Export': break; case 'Print': break } }
    function onSchedulerInit(s, e) { AddAdjustmentDelegate(adjustScheduler) }
    function adjustScheduler() {
        if (scheduler.GetActiveViewType() !== ASPxSchedulerViewType.Month) { if (window.innerHeight > 640 && window.innerWidth > 640 || scheduler.GetActiveViewType() === ASPxSchedulerViewType.Agenda) { var footerWrapperHeight = document.getElementById('footerWrapper').offsetHeight, height = window.innerHeight - schedulerToolbar.GetHeight() - footerWrapperHeight - headerPanel.GetHeight(); scheduler.SetHeight(height) } else scheduler.resetHeight() }
        scheduler.AdjustControl()
    }
    function onDateNavigatorSelectionChanged(s, e) { HideLeftPanelIfRequired() }
    var postponedCallbackRequired = !1; function onResourcesListBoxSelectedIndexChanged(s, e) {
        if (scheduler.InCallback())
            postponedCallbackRequired = !0; else scheduler.Refresh()
    }
    function onSchedulerEndCallback(s, e) { if (postponedCallbackRequired) { scheduler.Refresh(); postponedCallbackRequired = !1 } }
    function onSchedulerBeginCallback(s, e) { e.customArgs.SelectedResources = resourcesListBox.GetSelectedValues().join(',') }
    window.onSchedulerBeginCallback = onSchedulerBeginCallback; window.onSchedulerInit = onSchedulerInit; window.onSchedulerToolbarItemClick = onSchedulerToolbarItemClick; window.onDateNavigatorSelectionChanged = onDateNavigatorSelectionChanged; window.onResourcesListBoxSelectedIndexChanged = onResourcesListBoxSelectedIndexChanged; window.onSchedulerEndCallback = onSchedulerEndCallback
})()