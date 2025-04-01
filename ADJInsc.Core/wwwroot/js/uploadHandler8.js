//$(function () {
//    let selectedFiles = [];

//    $("#fileInput").on("change", function (event) {
//        let previewContainer = $("#previewContainer");
//        previewContainer.empty(); // Limpiar previos
//        selectedFiles = Array.from(event.target.files);

//        if (selectedFiles.length > 0) {
//            $("button[type='submit']").prop("disabled", false); // Habilitar botón de subida
//        } else {
//            $("button[type='submit']").prop("disabled", true);
//        }

//        selectedFiles.forEach((file, index) => {
//            let reader = new FileReader();
//            reader.onload = function (e) {
//                let fileWrapper = $(`
//                    <div style="position: relative; width: 100px; height: 100px; display: inline-block; margin: 5px;">
//                        <img src="${e.target.result}" data-index="${index}" class="file-thumbnail"
//                             style="width: 100px; height: 100px; object-fit: cover; border: 2px solid #ccc; cursor: pointer;">
//                        <div class="progress-bar" style="position: absolute; bottom: 0; left: 0; width: 0%; height: 5px; background-color: green;"></div>
//                    </div>
//                `);

//                previewContainer.append(fileWrapper);
//            };
//            reader.readAsDataURL(file);
//        });
//    });
//    $("#uploadForm").on("submit", function (event) {
//        event.preventDefault(); // Evita el envío tradicional

//        let uploadUrl = $(this).data("upload-url");

//        if (selectedFiles.length === 0) {
//            bootbox.alert("No hay archivos seleccionados.");
//            return;
//        }

//        $("button[type='submit']").prop("disabled", true); // Deshabilita el botón para evitar múltiples envíos

//        let formData = new FormData();

//        // Agregar todos los archivos a `formData`
//        selectedFiles.forEach((file) => {
//            formData.append("FilesModel", file); // El nombre debe coincidir con el backend
//        });

//        // Obtener el radio seleccionado y su label
//        let selectedRadio = $("input[name='ingresos']:checked");
//        let labelText = selectedRadio.length ? $("label[for='" + selectedRadio.attr("id") + "']").text() : "";

//        formData.append("TipoIngreso", labelText); // Agregar el valor seleccionado

//        $.ajax({
//            url: uploadUrl,
//            data: formData,
//            type: "POST",
//            contentType: false,
//            processData: false,
//            xhr: function () {
//                let xhr = new window.XMLHttpRequest();
//                xhr.upload.onprogress = function (e) {
//                    if (e.lengthComputable) {
//                        let percent = (e.loaded / e.total) * 100;
//                        $(".progress-bar").css("width", percent + "%");
//                    }
//                };
//                return xhr;
//            },
//            beforeSend: function () {
//                $("#AjaxLoader").show();
//            },
//            success: function (result) {
//                $('#finadhesion-link').removeClass('disabled');
//                //bootbox.alert(result.message);
//            },
//            error: function () {
//                bootbox.alert("Error al subir los archivos.");
//            },
//            complete: function () {
//                $("#AjaxLoader").hide();
//                $("button[type='submit']").prop("disabled", false);
//            }
//        });
//    });

//});

$(function () {
    let selectedFile = null; // Variable para almacenar un solo archivo

    $("#fileInput").prop("multiple", false); // Asegura que solo se permita un archivo

    $("#fileInput").on("change", function (event) {
        let previewContainer = $("#previewContainer");
        previewContainer.empty(); // Limpiar previos
        let files = event.target.files;

        if (files.length > 0) {
            selectedFile = files[0]; // Solo permitimos un archivo
            $("button[type='submit']").prop("disabled", false); // Habilitar botón de subida

            let reader = new FileReader();
            reader.onload = function (e) {
                let fileWrapper = $(`
                    <div style="position: relative; width: 100px; height: 100px; display: inline-block; margin: 5px;">
                        <img src="${e.target.result}" class="file-thumbnail" 
                             style="width: 100px; height: 100px; object-fit: cover; border: 2px solid #ccc; cursor: pointer;">
                        <div class="progress-bar" style="position: absolute; bottom: 0; left: 0; width: 0%; height: 5px; background-color: green;"></div>
                    </div>
                `);

                previewContainer.append(fileWrapper);
            };
            reader.readAsDataURL(selectedFile);
        } else {
            selectedFile = null;
            $("button[type='submit']").prop("disabled", true); // Deshabilita si no hay archivo
        }
    });

    $("#uploadForm").on("submit", function (event) {
        event.preventDefault(); // Evita el envío tradicional

        let uploadUrl = $(this).data("upload-url");

        if (!selectedFile) {
            bootbox.alert("No hay archivo seleccionado.");
            return;
        }

        $("button[type='submit']").prop("disabled", true); // Deshabilita el botón para evitar múltiples envíos

        let formData = new FormData();
        formData.append("FilesModel", selectedFile); // Solo un archivo permitido

        let selectedRadio = $("input[name='ingresos']:checked");
        let labelText = selectedRadio.length ? $("label[for='" + selectedRadio.attr("id") + "']").text() : "";

        formData.append("TipoIngreso", labelText); // Agregar el valor seleccionado

        $.ajax({
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
                        $(".progress-bar").css("width", percent + "%");
                    }
                };
                return xhr;
            },
            beforeSend: function () {
                $("#AjaxLoader").show();
            },
            success: function (result) {
                $('#finadhesion-link').removeClass('disabled');
                // bootbox.alert(result.message);
            },
            error: function () {
                bootbox.alert("Error al subir el archivo.");
            },
            complete: function () {
                $("#AjaxLoader").hide();
                $("button[type='submit']").prop("disabled", false);
            }
        });
    });
});
