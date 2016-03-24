function AdminViewModel() {
    var self = this;
    
    self.FormFields = ko.observableArray();
    self.FormName = ko.observable();
    self.nameClass = ko.observable("");

    self.toggleFieldOptions = ko.observable(true);
    self.optionVal = ko.observable();

    //Vars to create unique name val in form
    self.radioNum = 0;
    self.textNum = 0;
    self.checkNum = 0;
    self.dateNum = 0;
    self.timeNum = 0;
    self.dropNum = 0;


    self.Form = document.createElement('div');
    self.Form.setAttribute("id", "Form");

    self.RemoveField = function (item) {
        self.FormFields.remove(item);
    }

    //Event to add Text Field to FormFields
    self.AddTextField = function () {
        var tempField = new Text();
        var nameVal = tempField.Type() + ++self.textNum;
        tempField.Name(nameVal);
        self.FormFields.push(tempField);
    }

    //Event to add Header to FormFields
    self.AddHeader = function () {
        var tempHeader = new Header();
        self.FormFields.push(tempHeader);
    }

    //Event to add checkbox to FormFields
    self.AddCheckBox = function () {
        var tempCheck = new Checkbox();
        var nameVal = tempCheck.Type() + ++self.checkNum;
        tempCheck.Name(nameVal);
        self.FormFields.push(tempCheck);
    }

    self.AddDropdown = function () {
        var tempDrop = new Dropdown();
        var nameVal = tempDrop.Type() + ++self.dropNum;
        tempDrop.Name(nameVal);
        self.FormFields.push(tempDrop);
    }

    self.AddRadioGroup = function () {
        var tempRadio = new RadioGroup();
        var nameVal = tempRadio.Type() + ++self.radioNum;
        tempRadio.Name(nameVal);
        self.FormFields.push(tempRadio);
    }

    self.AddDatePicker = function () {
        var tempDate = new DateField();
        var nameVal = tempDate.Type() + ++self.dateNum;
        tempDate.Name(nameVal);
        self.FormFields.push(tempDate);
        $('.datepicker').datepicker();
        $('.datepicker').datepicker();
        $(".datepicker").datepicker("option", "showAnim", "slide");
    }

    self.AddTimePicker = function () {
        var tempTime = new TimeField();
        var nameVal = tempTime.Type() + ++self.timeNum;
        tempTime.Name(nameVal);
        self.FormFields.push(tempTime);
        $('.timepicker').timepicker();
    }

    self.SubmitForm = function () {
        if(self.FormFields().length <= 0)
        {
            bootbox.alert("Your form cannot be empty.");
            $("#formName").removeClass("has-error", 0, 0, 0);
        }
        else if(!self.FormName())
        {
            bootbox.alert("The form must have a name.");
            $("#formName").addClass("has-error", 0, 0, 0);
        }
        else {
            $("#formName").removeClass("has-error", 0, 0, 0);
            self.nameClass("");
            for (var i = 0; i < self.FormFields().length; i++) {
                self.Form.appendChild(self.FormFields()[i].CreateElement());
                if (self.FormFields()[i].Type() === "header") {
                    self.Form.appendChild(document.createElement("hr"));
                }
            }
            self.Form = self.Form.innerHTML;
            $.ajax({
                type: "POST",
                url: "/Admin/AddForm",
                data: {
                    formName: self.FormName(),
                    formHtml: self.Form,
                    workflow: 0
                },
                success: function (data) {
                    bootbox.alert(data.Message);
                },
                error: function (data) {
                    bootbox.alert(data.Message);
                }
            });
            self.Form = document.createElement('div');
            self.Form.setAttribute("id", "Form");
        }
    }
}