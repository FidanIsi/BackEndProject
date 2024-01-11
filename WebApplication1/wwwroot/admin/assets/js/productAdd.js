const imgInput = document.querySelector('.img-input');
const imgPreviewContainer = document.querySelector('.img-preview-container');

imgInput.addEventListener('change', (e) => {
    imgPreviewContainer.innerHTML = '';

    for (const file of e.target.files) {
        const imgDiv = document.createElement('div');
        imgDiv.setAttribute('class', 'img-preview');

        const img = document.createElement('img');
        img.style.width = "200px";
        const blobUrl = URL.createObjectURL(file);
        img.setAttribute('src', blobUrl);
        imgDiv.appendChild(img);

        imgPreviewContainer.appendChild(imgDiv);
    }
});
