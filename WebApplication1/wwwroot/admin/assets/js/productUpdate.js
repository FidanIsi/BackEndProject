const imgInput = document.querySelector('.img-input');
const imgPreviewContainer = document.querySelector('.img-preview-container');

imgInput.addEventListener('change', (e) => {
    imgPreviewContainer.innerHTML = '';

    for (const file of e.target.files) {
        const imgDiv = document.createElement('div');
        imgDiv.setAttribute('class', 'img-preview');

        const img = document.createElement('img');
        img.style.width = "100%";
        const blobUrl = URL.createObjectURL(file);
        img.setAttribute('src', blobUrl);
        imgDiv.appendChild(img);

        imgPreviewContainer.appendChild(imgDiv);
    }
});

const deleteButtons = document.querySelectorAll('.delete-btn');

deleteButtons.forEach((deleteBtn) => {
    deleteBtn.addEventListener('click', (e) => {
        const imgPreview = deleteBtn.previousElementSibling;

        imgPreview.src = null;
        imgPreview.remove();

        deleteBtn.remove();
    });
});

$(document).ready(function () {
    $('.delete-btn').on('click', function () {
        var imageId = $(this).data('imageid');
        $('<input>').attr({
            type: 'hidden',
            name: 'DeletedImageIds',
            value: imageId
        }).appendTo('form');
        $(this).closest('.img-preview').remove();
    });
});