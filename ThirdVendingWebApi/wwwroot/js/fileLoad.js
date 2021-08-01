function initFilePanel() {
    var sizeLimit = 314572800; //300 Mb
    const accStr = sessionStorage.getItem(accountKey);
    if (!accStr) {
        window.location.replace("/Account/Login");
    }
    const acc = JSON.parse(accStr);
    const role = acc.role;

    //get approve size
    $.getJSON("/api/appSettings", null)
    .then(function (data) {
        sizeLimit = data.fileMaxUploadLenght * 1024 * 1024;
    });

    $("#chkFile").change(function () {
        document.getElementById("deleteFileBtn").disabled = !$(this).prop("checked");
    });
    var admin = (role.toLowerCase().indexOf("super") >= 0 || role.toLowerCase() === "admin");
    document.getElementById("div_files").innerHTML = null;
    $("#progress .bar").css("width", "0");

    var wdz = 32;
    if (admin) wdz = 96;
    $("#fileLoadTable").dxDataGrid({
        dataSource: [],
        showColumnLines: false,
        showRowLines: true,
        keyExpr: "id",
        columnAutoWidth: true,
        columnFixing: { enabled: false },
        cellHintEnabled: true,
        showBorders: true,
        wordWrapEnabled: false,
        selection: { mode: "none" },
        focusedRowEnabled: true,
        height: function () {
                return (window.innerHeight / 1.4) - 46;
            },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        loadPanel: { enabled: true },
        paging: { pageSize: 10 },
        pager: {
            infoText: "Страница {0} из {1} ({2} файлов)",
            showInfo: true,
            visible: "always"
        },
        headerFilter: { visible: true },
        //onCellHoverChanged(e) {
        //    if (e.rowType === "data" && e.column.dataField === "CompanyName") {
        //        if (e.eventType === "mouseover") {
        //            this.TooltipTarget = e.cellElement;
        //            this.ToolTipText = e.text;
        //            this.tooltip.instance.show();
        //        } else {
        //            this.tooltip.instance.hide();
        //        }
        //    }
        //},
        columns: [
            {
                //width: "20%",
                allowSorting: true,
                allowHeaderFiltering: false,
                dataField: "fileDate",
                caption: "Дата",
                dataType: "date",
                format: "dd.MM HH:mm",
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            },
            {
                dataField: "name",
                allowSorting: true,
                allowHeaderFiltering: false,
                caption: "Имя/Описание",
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    },
                cellTemplate: function (element, info) {
                    element.append(`<div>${info.value}</div>`);
                    element.append(`<div>${info.data.description}</div>`);
                }
            },
            {
                width: wdz,
                fixed: false,
                allowSorting: true,
                type: "buttons",
                alignment: "center",
                cssClass: "vert-btn-align",
                caption: "",
                buttons: [
                    {
                        hint: "Изменить описание",
                        visible: function () {
                            return admin;
                        },
                        cssClass: "btn btn-primary btn-sm btn-font text-white fa fa-pencil",
                        onClick: function (e) {
                            const file = e.row.data;
                            const id = file.id;
                            if (!id) return;

                            document.getElementById("editFileId").value = file.id;
                            document.getElementById("fileDsc").value = file.description;
                            const editError = document.getElementById("editFileError");
                            editError.style.display = "none";
                            editError.innerText = null;
                            $("#editFilePopup").modal("toggle");
                        }
                    },
                    {
                        hint: "Скачать",
                        cssClass: "btn btn-primary btn-sm btn-font text-white fa fa-download",
                        onClick: function (e) {
                            const file = e.row.data;
                            const id = file.id;
                            if (!id) return;

                            window.location = `/api/files/${id}`;
                        }
                    },
                    {
                        hint: "Удалить",
                        visible: function () {
                            return admin;
                        },
                        cssClass: "btn btn-danger btn-sm btn-font text-white fa fa-remove", //fa fa-window-close text-danger",
                        onClick: function (e) {
                            const file = e.row.data;
                            const id = file.id;
                            if (!id) return;

                            const messageHtml = `<div>Вы действительно хотите удалить</div>${`<div>файл "${file.name}" из системы?</div>`}`;
                            document.getElementById("deleteFileCaption").innerHTML = messageHtml;
                            document.getElementById("deleteFileId").value = file.id;
                            const delError = document.getElementById("deleteFileError");
                            $("#chkFile").prop("checked", false);
                            
                            delError.style.display = "none";
                            delError.innerText = null;
                            $("#deleteFilePopup").modal("toggle");
                        }
                    }
                ]
            },
            {
                width: 38,
                dataField: "visible",
                allowSorting: true,
                alignment: "center",
                cssClass: "vert-btn-align",
                caption: null,
                visible: admin,
                cellTemplate: function (element, info) {
                    const file = info.data;
                    var dx;
                   
                    if (file.visible) dx = '<a href="#" class="btn btn-success btn-sm btn-font fa fa-eye" onclick="setFileVisible(false)" data-toggle="tooltip" title="Сделать невидимым"></a>';
                    else dx = '<a href="#" class="btn btn-secondary btn-sm btn-font fa fa-eye-slash" onclick="setFileVisible(true)" data-toggle="tooltip" title="Сделать видимым"></a>';
                    element.append(dx);
                }
                ,headerFilter: {
                    dataSource: [
                        {
                            text: "Видимые",
                            value: ["visible", "=", true],
                            template: function () {
                                return '<i class="fa fa-eye text-success" aria-hidden="true"></i>&nbsp;Видимые';
                            }
                        },
                        {
                            text: "Невидимые",
                            value: ["visible", "=", false],
                            template: function () {
                                return '<i class="fa fa-eye-slash text-secondary" aria-hidden="true"></i>&nbsp;Невидимые';
                            }
                        }
                    ]
                }
            }
            
        ]
    });

    getFiles();

    if (admin) {
        document.getElementById("fileUploadWidget").style.display = "block";
        $("#fileUploadWidget").fileupload({
            singleFileUploads: true,
            autoUpload: false,
            beforeSend: function(xhr, data) {
                const token = sessionStorage.getItem(tokenKey);
                xhr.setRequestHeader("Authorization", `Bearer ${token}`);
            },
            add: function(e, data) {
                const uploadErrors = [];
                //var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
                //if(data.originalFiles[0]['type'].length && !acceptFileTypes.test(data.originalFiles[0]['type'])) {
                //    uploadErrors.push('Not an accepted file type');
                //}
                
                const fileSize = data.originalFiles[0]["size"];
                if (fileSize && fileSize > sizeLimit) {
                    uploadErrors.push("Filesize is too big");
                }
                if (uploadErrors.length > 0) {
                    alert(uploadErrors.join("\n"));
                } else {
                    data.submit();
                }
            },
            done: function(e, data) {
                const file = data.result.files;
                $(`<p style="color: green;">${file.name}<i class="elusive-ok" style="padding-left:10px;"/> - Size: ${
                        file.size} byte</p>`)
                    .appendTo("#div_files");
                getFiles();
                //$.each(data.result.files, function (index, file) {
                //    $('<p style="color: green;">' + file.name + '<i class="elusive-ok" style="padding-left:10px;"/> - Type: ' + file.type + ' - Size: ' + file.size + ' byte</p>')
                //        .appendTo('#div_files');
                //});
            },
            fail: function(e, data) {
                $.each(data.messages,
                    function(index, error) {
                        $(`<p style="color: red;">Upload file error: ${error
                                }<i class="elusive-remove" style="padding-left:10px;"/></p>`)
                            .appendTo("#div_files");
                    });
            },
            progressall: function(e, data) {
                const progress = parseInt(data.loaded / data.total * 100, 10);

                $("#progress .bar").css("width", progress + "%");
            }
        });
    } else {
        document.getElementById("fileUploadWidget").style.display = "none";
    }
}

function getFiles() {
    $.getJSON("/api/files", null)
        .then(function (data) {
            $("#fileLoadTable").dxDataGrid("instance").option("dataSource", data);
        });
}

function setFileVisible(visible) {
    const dataGrid = $("#fileLoadTable").dxDataGrid("instance");
    const rowKey = dataGrid.option("focusedRowKey");

    $.putJSON(`/api/files/setVisible/${rowKey}`, visible ? true: false, null).then(function() {
        getFiles();
        //return;
    });
}

function deleteFile() {
    const id = document.getElementById("deleteFileId").value;
    var fileError = document.getElementById("deleteFileError");
    fileError.style.display = "none";
    $.deleteObj(`/api/files/${id}`).then(function() {
            $("#deleteFilePopup").modal("hide");
            getFiles();
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                $("#deleteFilePopup").modal("hide");
                getFiles();
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;

                fileError.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                fileError.style.display = "block";
            }
        });
}

function editFile() {
    const id = document.getElementById("editFileId").value;
    var fileError = document.getElementById("editFileError");
    fileError.style.display = "none";

    const dsc = document.getElementById("fileDsc").value;
    $.putJSON(`/api/files/${id}`, dsc, null).then(function() {
            $("#editFilePopup").modal("hide");
            getFiles();
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                $("#editFilePopup").modal("hide");
                getFiles();
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;

                fileError.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                fileError.style.display = "block";
            }
        });
}