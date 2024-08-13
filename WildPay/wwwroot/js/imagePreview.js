/**
 * Previews user's uploaded image and hides the default image;
 * Checks if the image is valid before preview / submit.
 * @param {string} inputId
 * @param {string} previewImageId
 * @param {string} defaultImageId
 * @param {string} errorId
 * @param {string} submitButtonid
 * @returns {void}
 */
function setupImagePreview(inputId, previewImageId, defaultImageId, errorId, submitButtonid) {
    //retrieve the elements with their ID
    let input = document.getElementById(inputId);
    let previewImage = document.getElementById(previewImageId);
    let defaultImage = document.getElementById(defaultImageId);
    let fileError = document.getElementById(errorId);
    let submitButton = document.getElementById(submitButtonId);

    // if the user loads an image as a new profile picture
    input.addEventListener('change', (event) => {
        let file = event.target.files[0];

        // reset the preview image and error message
        previewImage.style.display = 'none';
        previewImage.src = "#";
        fileError.style.display = 'none';
        fileError.textContent = '';
        defaultImage.style.display = 'block';

        // submit button enabled by default
        submitButton.disabled = false;

        if (file) {
            if (!IsTypeValid(file)) {
                fileError.textContent = "L'image doit être au format jpeg, jpg ou png";
                fileError.style.display = 'block';
                input.value = '';

                submitButton.disabled = true;
                return;
            }

            // checking image's size
            if (!IsSizeValid(file.size)) {
                fileError.textContent = "La taille maximale de l'image autorisée est de 2Mo";
                fileError.style.display = 'block';
                input.value = '';

                submitButton.disabled = true;
                return;
            }

            // FileReader = object to read the file's data.
            var reader = new FileReader();

            reader.onload = function (e) {
                previewImage.src = e.target.result;
                previewImage.style.display = 'block';
                defaultImage.style.display = 'none';
            }

            // converts the file to a data URL,
            // which can be used as the src of an image
            reader.readAsDataURL(file);
        }
    })
}

/**
 * Checks if the file's type is valid.
 * @param {File} file
 * @returns {boolean}
 */
function IsTypeValid(file) {
    const fileType = file.type.toLowerCase();
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];

    if (validTypes.includes(fileType)) {
        return true;
    }

    return false;
}

/**
 * Checks if the file's size is valid.
 * @param {number} fileSize
 * @returns {boolean}
 */
function IsSizeValid(fileSize) {
    const maxsize = 2 * 1024 * 1024;

    if (fileSize > maxSize) {
        return false;
    }

    return true;
}