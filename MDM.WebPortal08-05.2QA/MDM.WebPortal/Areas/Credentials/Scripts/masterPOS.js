/*-----------------EVENTS HANDLER FUNCTIONS----------------------*/

/*Se captura el evento que se dispara al cambiar la BD en DatabaseDrpDownEditor.cshtml cada vez que se va a crear o editar un MasterPOS, 
  mostrando solo los POSIDs relacionados con el DB seleccionado*/
function onChange() {
    $("#PosIDs").data('kendoMultiSelect').dataSource.read();
}

/*Se captura el evento que se dispara al modificar el DataSource del Grid principal (MasterPOS), especificamente cuando se modifica el campo 
  FvPList_FvPID, en dependencia del valor que este tome se mostraran ciertos tabs, Ex: Si el campo cambia a "PHY" o "LAB" entonces se muestra 
  el tab PHYSICIAN GROUP y se ocultan LICENSE y LEVEL OF CARE*/
function onChangeInside(e) {
    if (e.action === "itemchange") { //primero me cersioro de que sea mientras se esta haciendo un update
        if (e.field === "FvPList_FvPID") { //especificamente cuando se esta modificando el tipo de POS: "FAC" or "PHY"
            var model = e.items[0]; //Obtengo el row completo del objeto que se esta actualizando
            var type = model.get("FvPList_FvPID"); //obtengo el PK del nuevo tipo seleccionado, en este caso FAC es 1 y PHY es 2
            var pk = model.get("MasterPOSID"); //obtengo el valor del primary key del correspondiente LocationPOS object

            if (type === 2 || type === 3) { //Si el tipo de POS es PHY entonces muestro el tab 2: "Physician Group"
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[6]).attr("style", "display:true");
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[5]).attr("style", "display:none");
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[1]).attr("style", "display:none");
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[3]).attr("style", "display:none");
            } else {
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[6]).attr("style", "display:none");
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[5]).attr("style", "display:true");
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[1]).attr("style", "display:true");
                $($("#tabStrip_" + pk).data("kendoTabStrip").items()[3]).attr("style", "display:true");
            }
        }
    }
}

/*Se captura el Evento que se dispara cada vez que se expande una fila para acceder a los detalles de la misma*/
function detailExpand(e) {
    var rowPK = e.sender.dataItem(e.masterRow).MasterPOSID; //Obtengo el row completo del objeto que se esta actualizando
    var type = e.sender.dataItem(e.masterRow).FvPList_FvPID; //obtengo el PK del nuevo tipo seleccionado, en este caso FAC es 1 y PHY es 2
    if (type === 1) {
        //alert("FAC");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[6]).attr("style", "display:none");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[1]).attr("style", "display:true");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[3]).attr("style", "display:true");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[5]).attr("style", "display:true");
    } else {
        //alert("PHY");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[6]).attr("style", "display:true");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[1]).attr("style", "display:none");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[3]).attr("style", "display:none");
        $($("#tabStrip_" + rowPK).data("kendoTabStrip").items()[5]).attr("style", "display:none");
    }
}

/*Captura el evento que se dispara al seleccionar cada uno de los tabs del componente TabStrip que muestra los detalles de cada uno de los
  los objetos MasterPOS, de modo tal que se obtiene solo aquellos tabs que contienen Grids y se pide que se lea la fuente de cada uno de 
  ellos
 */
function onSelectTab(e) {
    console.log("Selected: " + $(e.item).find("> .k-link").text());
    console.log("Selected: " + $(e.contentElement).find("> .k-grid").attr('id'));
    var gridId = $(e.contentElement).find("> .k-grid").attr('id');
    if (gridId != undefined) {
        var grid = $("#" + gridId).data("kendoGrid");
        grid.dataSource.read();
    }
}
/*--------------------------------------------------------------*/


/*-----------------ACTIVATE WINDOWS FUNCTIONS-------------------*/
function customCommand(e) {
    $("#windowChoose_" + e).data("kendoWindow").open().center();
}

function openWindow(h) {
    $("#wdwGrp_" + h).data("kendoWindow").open().center();
}

function activeWdwPosFile(z) {
    $("#wdwNewFile_" + z).data("kendoWindow").open().center();
}
/*--------------------------------------------------------------*/


/*-----------------AJAX REQUEST FUNCTIONS-----------------------*/
function sendPhyGrp(t) {
    var drpdown = $("#phyGrp_" + t).data("kendoDropDownList");
    var dataItem = drpdown.dataItem();

    var domainName = "http://" + window.location.host.toString();
    $.ajax({
        type: "POST",
        url: domainName + '/PHYGroups/Assign_PhyGrpToPos',
        data: {
            MasterPOSID: t,
            PHYGrpID: dataItem.PHYGrpID
        },
        success: function (data) {
            if (data.length === 0) {
                alert("Something failed. Please try again!");
            }
            $("#wdwGrp_" + t).data("kendoWindow").close();
            var listV = $("#lV_" + t).data("kendoListView");
            listV.dataSource.read();
            //location.reload();
        },
        complete: function (xhr, success) { },
        error: function (xhr, ajaxOptions, thrwnError) {
            alert('Something failed. Please try again!');
        }
    });
}

function CheckRequired(d) {
    var multiselect = $("#corpCnt_" + d).data("kendoMultiSelect");
    var list = multiselect.dataItems();
    if (list.length === 0) {
        $("#corpCnt_9").notify("You have to select at least one Contact. Try again!",
            {
                position: "bottom left",
                className: 'error'
            }
        );
    } else {
        var domainName = "http://" + window.location.host.toString();
        var info = [];
        for (i = 0; i < list.length; i++) {
            info.push(list[i].ContactID);
        }
        $.ajax({
            type: "POST",
            url: domainName + '/MasterPOS_Contact/Save_POSContacts',
            data: {
                MasterPOSID: d,
                Contacts: info
            },
            success: function (data) {
                if (data.length === 0) {
                    alert("Something failed. Please try again!");
                }
                $("#windowChoose_" + d).data("kendoWindow").close();
                var grid = $("#PosContactGrid_" + d).data("kendoGrid");
                grid.dataSource.read();

            },
            complete: function (xhr, success) { },
            error: function (xhr, ajaxOptions, thrwnError) {
                alert('Something failed. Please try again!');
            }
        });
    }
}//Sending MasterPOS_Contacts to the server side

function UploadPosFile(g) {

    var drpdown = $("#FileTypeID_" + g).data("kendoDropDownList");
    var dataItem = drpdown.dataItem();
    var description = $("#tbDes_" + g).val();
    var domainName = "http://" + window.location.host.toString();

    var input = document.querySelector('#fichero');
    var files = input.files;
    //var upload = $("#fichero").data("kendoUpload"),
    //      files = upload.getFiles();
    alert(files[0].name + files[0].extension + " " + files[0].size);

    if (files.length > 0) {
       
        if (window.FormData !== undefined) {

            var data = new FormData();

            data.append("MasterPOSID", g);
            data.append("FileTypeID", dataItem.FileTypeID);
            data.append("Description", description);
            data.append("fichero", files[0], files[0].name);

            var reader = new FileReader();
            reader.readAsArrayBuffer(files[0]);
            console.log(reader.result);

            $.ajax({
                type: "POST",
                url: domainName + '/POSFiles/UploadFileWAjax',
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    console.log(result);
                    $("#wdwNewFile_" + g).data("kendoWindow").close();
                    var listV = $("#lvFiles_" + g).data("kendoListView");
                    listV.dataSource.read();
                },
                error: function (xhr, status, p3, p4) {
                    var err = "Error " + " " + status + " " + p3 + " " + p4;
                    if (xhr.responseText && xhr.responseText[0] == "{")
                        err = JSON.parse(xhr.responseText).Message;
                    console.log(err);
                }
            });
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    }
}
/*--------------------------------------------------------------*/


/*-----------------ERROR HANDLER FUNCTIONS----------------------*/
function posName_errorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var grid = $("#AllPOS").data("kendoGrid");
        grid.cancelChanges();
    }
}

function Lev_errorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var grid = jQuery("[id^='gridLevel_']").data("kendoGrid");
        grid.cancelChanges();
    }
}

function ServError_handler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var grid = jQuery("[id^='gridService_']").data("kendoGrid");
        grid.cancelChanges();
    }
}

function POSContact_ErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var grid = jQuery("[id^='PosContactGrid_']").data("kendoGrid");
        grid.cancelChanges();
    }
}

function formsSent_ErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var grid = jQuery("[id^='FormsGrid_']").data("kendoGrid");
        grid.cancelChanges();
    }
}

function extraInfoDetailErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var lv = jQuery("[id^='lvInfo_']").data("kendoListView");
        //var lv = $("#lvInfo_" + e.MasterPOSID).data("kendoListView");
        lv.refresh();
    }
}

function extraQuestionErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var lv = jQuery("[id^='lvQuestion_']").data("kendoListView");
        lv.cancel();
    }
}

function addressInfoErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var lv = jQuery("[id^='lVAddress_']").data("kendoListView");
        lv.cancel();
    }
}

function licenseInfoErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var lv = jQuery("[id^='listVLicense_']").data("kendoListView");
        lv.cancel();
    }
}

function facInfoErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var lv = jQuery("[id^='lvFacility_']").data("kendoListView");
        lv.cancel();
    }
}

function phyErrorHandler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        var lv = jQuery("[id^='lV_']").data("kendoListView");
        lv.cancel();
    }
}
/*--------------------------------------------------------------*/


/*-----------------REQUEST END EVENTS FUNCTIONS----------------*/
/*Captura evento que se dispara especificamente al modificar un contacto, especificamente la propiedad Active,
  si el valor pasa a ser falso entonces dicho contacto no debe mostrarse en su respectivo Grid
 */
function editPosCnt(e) {
    var type = e.type;
    if (type === "update") {
        var response = e.response.Data[0].active;
        if (response === false) {
            this.read(); //calling the read() of the current dataSource that allows you just display the active contacts
        }
    }
}
/*--------------------------------------------------------------*/


/*-----------------SIGNALR FUNCTIONS----------------------------*/
$(function notifyDuplications() {
    var connection = $.hubConnection();
    var hub = connection.createHubProxy("MasterPOS");
    hub.on("notifyIfDuplicatePosIDs", function (notificacion) {
        $.notify(notificacion);
    });
    connection.start(function () {
        hub.invoke("doIfDuplicatePosIds");
    });
});

$(function notifyReleaseDb() {
    var connection = $.hubConnection();
    var hub = connection.createHubProxy("UpdatePOS");
    hub.on("notifyIfRelease", function () {
        var grid = $("#AllPOS").data("kendoGrid");
        grid.dataSource.read();
    });
    connection.start(function () {
        hub.invoke("doIfReleaseDB");
    });
});

//$(function () {
//    var connection = $.hubConnection();
//    var hub = connection.createHubProxy("UpdatePOS");
//    hub.on("notifyIfChangePos", function (i, n, a) {
//        var grid = $("#AllPOS").data("kendoGrid");
//        grid.dataSource.pushUpdate({ MasterPOSID: i, PosMasterName: n, active: a });
//    });
//    connection.start(function () {
//        hub.invoke("doIfUpdatePosFromCorp");
//    });
//});

/*--------------------------------------------------------------*/