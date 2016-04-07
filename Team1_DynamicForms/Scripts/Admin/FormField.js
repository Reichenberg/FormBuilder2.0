var Text = function () {
    var self = this;
    self.Label = ko.observable("Label Text");
    self.IsRequired = ko.observable(false);
    self.Type = ko.observable("text");
    self.Name = ko.observable();

    //Create form element based on data
    self.CreateElement = function () {
        var formControl = document.createElement("div");
        formControl.setAttribute("class", "form-group");

        var label = document.createElement("label");
        label.setAttribute("class", "form-label");
        label.innerText = self.Label();

        var input = document.createElement("input");
        input.setAttribute("class", "form-control");
        input.setAttribute("type", self.Type());
        input.setAttribute("value", "");
        input.setAttribute("name", self.Name());
        input.required = self.IsRequired();

        //add label to form-control element
        formControl.appendChild(label);
        //add input to form-control element
        formControl.appendChild(input);
        
        return formControl;
    }
}

var Checkbox = function () {
    var self = this;
    self.Label = ko.observable("Label Text");
    self.IsRequired = ko.observable(false);
    self.Type = ko.observable("checkbox");
    self.Name = ko.observable();

    //Create form element based on data
    self.CreateElement = function () {
        var formControl = document.createElement("div");
        formControl.setAttribute("class", "checkbox-inline");
        var label = document.createElement("label");

        var input = document.createElement("input");
        input.setAttribute("type", self.Type());
        input.setAttribute("value", "");
        input.setAttribute("name", self.Name());

        //add input to label
        label.appendChild(input);
        label.innerHTML += self.Label();
        //add label to form control
        formControl.appendChild(label);

        return formControl;
    }
}

var Header = function () {
    var self = this;
    self.Value = ko.observable("Default");
    self.Type = ko.observable("header");

    //Create form element based on data
    self.CreateElement = function () {
        var formControl = document.createElement("h3");
        formControl.innerText = self.Value();

        return formControl;
    }
}

var Dropdown = function () {
    var self = this;
    self.Label = ko.observable("Label Text");
    self.Values = ko.observableArray();
    self.Type = ko.observable("dropdown");
    self.IsRequired = ko.observable(false);
    self.Selected = ko.observable();
    self.Name = ko.observable();

    //Default option for drop down
    self.Values.push("Select Option");

    self.AddOption = function (data, event) {
        self.Values.push(data);
    }

    //Create form element based on data
    self.CreateElement = function () {
        var formControl = document.createElement("div");
        formControl.setAttribute("class", "form-group");

        var label = document.createElement("label");
        label.setAttribute("class", "form-label");
        label.innerText = self.Label();
        //add label to form-control element
        formControl.appendChild(label);

        var drop = document.createElement("select");
        drop.setAttribute("class", "form-control");
        drop.setAttribute("name", self.Name());
        drop.required = self.IsRequired();

        //Adds default option for dropdown field
        var defaultOp = document.createElement("option");
        defaultOp.setAttribute("value", "");
        defaultOp.innerText = "Select Option";
        //add input to form-control element
        drop.appendChild(defaultOp);
       
        for (count = 1; count < self.Values().length; count++) {
            var tempInput = document.createElement("option");
            tempInput.setAttribute("value", self.Values()[count]);
            tempInput.innerText = self.Values()[count];
            //add input to form-control element
            drop.appendChild(tempInput);
        }
        //add the input to the form control
        formControl.appendChild(drop);
        return formControl;
    }
}

var RadioGroup = function () {
    var self = this;
    self.RadioList = ko.observableArray();
    self.Name = ko.observable();
    self.Label = ko.observable();
    self.IsRequired = ko.observable(false);
    self.Type = ko.observable("radio");

    self.AddOption = function (data, event) {

        //Check if name is unique before adding 
        //return alert
        var option = new RadioOption();
        option.Value(data);
        self.RadioList.push(option);
    }

    self.RemoveOption = function (item) {
        self.RadioList.remove(item);
    }

    self.CreateElement = function () {
        //empty div to organize radio buttons
        var formControl = document.createElement("div");
        var groupHeader = document.createElement("h3");
        groupHeader.innerText = self.Label();
        formControl.appendChild(groupHeader);
        
        for (var c = 0; c < self.RadioList().length; c++) {
            //radio holder
            var radioOption = document.createElement("div");
            radioOption.setAttribute("class", "radio");
            var label = document.createElement("label");
            
            //set up radio input with appropriate values
            var radio = document.createElement("input");
            radio.setAttribute("type", self.Type());
            radio.setAttribute("value", self.RadioList()[c].Value());
            radio.setAttribute("name", self.Name());

            radio.required = self.IsRequired();

            //add radio to label
            label.appendChild(radio);

            label.innerHTML += self.RadioList()[c].Value();
            //add label to radio holder
            radioOption.appendChild(label);
            //add the radio holder to the container
            formControl.appendChild(radioOption);
        }
        return formControl;
    }
}

var RadioOption = function () {
    var self = this;
    self.Value = ko.observable();
}

var DateField = function () {
    var self = this;
    self.Label = ko.observable();
    self.Type = ko.observable("datepicker");
    self.IsRequired = ko.observable(false);
    self.Name = ko.observable();

    //Create form element based on data
    self.CreateElement = function () {
        var formControl = document.createElement("div");
        formControl.setAttribute("class", "form-group");

        var label = document.createElement("label");
        label.setAttribute("class", "form-label");
        label.innerText = self.Label();

        var input = document.createElement("input");
        input.setAttribute("class", "form-control " + self.Type());
        input.setAttribute("type", self.Type());
        input.setAttribute("value", "");
        input.setAttribute("name", self.Name());
        input.required = self.IsRequired();

        //add label to form-control element
        formControl.appendChild(label);
        //add input to form-control element
        formControl.appendChild(input);

        return formControl;
    }
}

var TimeField = function () {
    var self = this;
    self.Label = ko.observable();
    self.Type = ko.observable("timepicker");
    self.IsRequired = ko.observable(false);
    self.Name = ko.observable();

    //Create form element based on data
    self.CreateElement = function () {
        var formControl = document.createElement("div");
        formControl.setAttribute("class", "form-group");

        var label = document.createElement("label");
        label.setAttribute("class", "form-label");
        label.innerText = self.Label();

        var input = document.createElement("input");
        input.setAttribute("class", "form-control " + self.Type());
        input.setAttribute("type", self.Type());
        input.setAttribute("value", "");
        input.setAttribute("name", self.Name());
        input.required = self.IsRequired();

        //add label to form-control element
        formControl.appendChild(label);
        //add input to form-control element
        formControl.appendChild(input);

        return formControl;
    }
}

