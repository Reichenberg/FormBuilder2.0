$(function () {
    var viewModel = new AdminViewModel();
    $("#textDrag").draggable({ snap: ".ui-widget-header" });

    //Applies the viewModel bindings to the page
    ko.applyBindings(viewModel);
    $(document).tooltip();
   
});