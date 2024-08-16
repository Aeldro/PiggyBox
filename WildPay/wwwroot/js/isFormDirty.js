//import { IsTypeValid } from "./imagePreview.js";
//import { IsSizeValid } from "./imagePreview.js";

function setupChangeDetection(formId, buttonId) {
    let form = document.getElementById(formId);
    let button = document.getElementById(buttonId);
    let initialValues = {};

    // store initial values of the form fields
    Array.from(form.elements).forEach(function (element) {
        if (element.tagName.toLowerCase() !== 'button'
            && element.type !== 'submit') {
            initialValues[element.name] = element.value;
        }
    });

    // inner function
    function isFormDirty() {
        for (var key in initialValues) {
            if (initialValues.hasOwnProperty(key)) {
                var element = form.elements[key];

                if (element && element.value !== initialValues[key]) {
                    if (element.id === "customFile") {
                        file = document.getElementById(element.id).files[0];

                        if (!IsImageValid(file)) {
                            continue;
                        }
                    }

                    return true;
                }
            }
        }
        return false;
    }

    // change event listener to each input field
    Array.from(form.elements).forEach(function (element) {
        if (element.tagName.toLowerCase() !== 'button'
            && element.type !== 'submit') {
            element.addEventListener('input', function () {
                button.disabled = !isFormDirty();
            });
        }
    });
}

function IsImageValid(file) {
    const fileType = file.type.toLowerCase();
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];

    const fileSize = file.size;
    const maxSize = 2 * 1024 * 1024;

    if (!validTypes.includes(fileType)) {
        return false;
    }

    if (fileSize > maxSize) {
        return false;
    }

    return true;
}