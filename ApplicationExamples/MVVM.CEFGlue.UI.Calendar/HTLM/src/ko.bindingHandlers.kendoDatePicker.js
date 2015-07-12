//------------------Date Picker Knockout Custom Binding
ko.bindingHandlers.kendoDatePicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var unwrap = ko.utils.unwrapObservable;
        var dataSource = valueAccessor();
        var binding = allBindingsAccessor();
        var options = {};
        var source;

        if (binding.datePickerOptions) {
            options = $.extend(options, binding.datePickerOptions);
        }

        if (dataSource) {
            var handleValueChange = function () {
                //change the knockout model object with the specified value
                var changeModel = function (value) {
                    if (ko.isWriteableObservable(dataSource)) {
                        //Since this is an observable, the update part will fire and select the 
                        //  appropriate display values in the controls
                        dataSource(value);
                    } else {  //write to non-observable
                        if (binding['_ko_property_writers'] && binding['_ko_property_writers']['kendoDatePicker']) {
                            binding['_ko_property_writers']['kendoDatePicker'](value);
                        }
                    }
                };

                //Get the selected Value from the Kendo ComboBox
                var selectedValue = this.value();
                //If they dont select anything, then there intent is to null out the value
                if (!selectedValue) {
                    changeModel(null);
                } else {
                    changeModel(selectedValue);
                }
                return false;
            };
            options.change = handleValueChange;
        }

        //handle the choices being updated in a Dependant Observable (DO), so the update function doesn't 
        // have to do it each time the value is updated. Since we are passing the dataSource in DO, if it is
        // an observable, when you change the dataSource, the dependentObservable will be re-evaluated
        // and its subscribe event will fire allowing us to update the autocomplete datasource
        var mappedSource = ko.dependentObservable(function () {
            return unwrap(dataSource);
        }, viewModel);
        //Subscribe to the knockout observable array to get new/remove items
        mappedSource.subscribe(function (newValue) {
            var datePicker = $(element).data('kendoDatePicker');
            if (datePicker.value() != newValue)
                datePicker.value(newValue);
        });

        options.value = mappedSource();
        $(element).kendoDatePicker(options);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //update value based on a model change
        var unwrap = ko.utils.unwrapObservable;
        var dataSource = valueAccessor();
        var binding = allBindingsAccessor();
        var valueProp = unwrap(binding.optionsValue);
        var labelProp = unwrap(binding.optionsText) || valueProp;

        if (dataSource) {
            var currentModelValue = unwrap(dataSource);
            if (dataSource)
                $(element).data('kendoDatePicker').value(currentModelValue);
            else
                $(element).data('kendoDatePicker').value('');
        }
    }
};
