$(function () {
    let selectedFiles = [];

    $("#fileInput").on("change", function (event) {
        let previewContainer = $("#previewContainer");
        previewContainer.empty(); // Limpiar previos
        selectedFiles = Array.from(event.target.files);

        if (selectedFiles.length > 0) {
            $("button[type='submit']").prop("disabled", false); // Habilitar botón de subida
        } else {
            $("button[type='submit']").prop("disabled", true);
        }

        selectedFiles.forEach((file, index) => {
            let reader = new FileReader();
            reader.onload = function (e) {
                let fileWrapper = $(`
                    <div style="position: relative; width: 100px; height: 100px; display: inline-block; margin: 5px;">
                        <img src="${e.target.result}" data-index="${index}" class="file-thumbnail" 
                             style="width: 100px; height: 100px; object-fit: cover; border: 2px solid #ccc; cursor: pointer;">
                        <div class="progress-bar" style="position: absolute; bottom: 0; left: 0; width: 0%; height: 5px; background-color: green;"></div>
                    </div>
                `);

                previewContainer.append(fileWrapper);
            };
            reader.readAsDataURL(file);
        });
    });

    $("#uploadForm").on("submit", function (event) {
        event.preventDefault(); // Evita envío tradicional

        let uploadUrl = $(this).data("upload-url");

        if (selectedFiles.length === 0) {
            bootbox.alert("No hay archivos seleccionados.");
            return;
        }

        $("button[type='submit']").prop("disabled", true); // 🔴 Evita envíos múltiples

        let uploadPromises = selectedFiles.map((file, index) => {
            let progressBar = $(`.file-thumbnail[data-index="${index}"]`).siblings(".progress-bar");
            return uploadFile(file, progressBar, uploadUrl);
        });

        // Espera a que todas las cargas terminen antes de habilitar el botón
        Promise.all(uploadPromises).finally(() => {
            $("button[type='submit']").prop("disabled", false);
        });
    });

});

function uploadFile(file, progressBar, uploadUrl) {
    let formData = new FormData();
    formData.append("FilesModel", file); // Debe coincidir con el backend

    return $.ajax({
        url: uploadUrl,
        data: formData,
        type: "POST",
        contentType: false,
        processData: false,
        xhr: function () {
            let xhr = new window.XMLHttpRequest();
            xhr.upload.onprogress = function (e) {
                if (e.lengthComputable) {
                    let percent = (e.loaded / e.total) * 100;
                    progressBar.css("width", percent + "%");
                }
            };
            return xhr;
        },
        beforeSend: function () {
            $("#AjaxLoader").show();
        },
        success: function (result) {
            $('#finadhesion-link').removeClass('disabled');
            //$("#AjaxLoader").delay(2000).hide("slow");
            //bootbox.alert(result.message);
        },
        error: function () {
            $("#AjaxLoader").delay(2000).hide("slow");
            bootbox.alert("Error al subir los archivos.");
        }
    });
}
