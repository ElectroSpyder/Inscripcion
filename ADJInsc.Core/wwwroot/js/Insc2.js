$(function () {
    $('#declaracionModal').modal('toggle');
});

$(function () {

    $("#declaracionClic").click(function () {
        $("#btnBuscar").prop("disabled", false);
        $("#declaracionModal").modal('hide');
    });

    //$("#declaracionAdhesionClic").click(function () {
    //    $("#declaracionAdhesionModal").modal('hide');
    //    $("#declaracionModal").modal('show');
    //});

    //$("#btnHideModalDeclaracion").click(function () {
        
    //    $("#declaracionAdhesionModal").modal('hide');
    //});

    $("#btnHideModalDeclaracion").click(function () {
        $("#btnBuscar").prop("disabled", true);
        $("#declaracionModal").modal('hide');
    });

    $("#btnHideModalRecuperar").click(function () {        
        $("#recuperarModal").modal('hide');
    });

    $("#btnLogin").click(function () {
        // $("#AjaxLoaderLogin").show("fast");
        $("#loginModal").modal('show');
    });

    $("#btnShowDeclaracion").click(function () {
        $('#declaracionModal').modal('show');
    });

    $("#btnShowRecuperar").click(function () {
        $('#recuperarModal').modal('show');
    });

    $("#btnHideModal").click(function () {
        $("#loginModal").modal('hide');
        $.ajax({
            url: '@Url.Action("GetLogin", "Inscripcion")',
            data: {
                userName: userName,
                userPassword: userPassword
            },
            type: "post", // Verbo HTTP
            timeout: 1500,

        }).done(function (data) {

        }).fail(function () {

        });
    });
});


function valideKey(evt)
{
    var x = event.which;
    
    if (x >= 48 && x <= 57) {
        //console.log('El caracter ingresado es un numero')
        return true;
    }
    else {
        //console.log('El caracter ingresado es un texto')
        return false;
    }
    
}
//$(function () {
//    $("#btnBuscar").click(function (e) {

//        var numeroDni = $("#dniInput").val();
        
//        $("#AjaxLoader").show("fast");

//        if (numeroDni.length > 6 && numeroDni.length < 9) {

//            $("#AjaxLoader").delay(2000).hide("slow");


//        } else {

//            e.preventDefault();
//            e.stopPropagation();

//            bootbox.alert({
//                size: "small",
//                title: "I.V.U.J.",
//                message: "Debes ingresar un Dni correcto",
//                callback: function () {
//                    $("#AjaxLoader").hide("slow");
//                }
//            });

//        }
//    });
//});

