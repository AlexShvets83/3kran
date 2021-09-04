function initAppSettings() {
    var appError = document.getElementById("appSettingsFormErrors");
    appError.style.display = 'none';
    $.getJSON("/api/appSettings", null)
        .then(function (data) {
            $("#appSettingsForm").dxForm({
                formData:data,
                showColonAfterLabel: true,
                labelLocation: "left", // or "left" | "right" "top"
                items: [
                    {
                        dataField: "userLogDepth",
                        editorType: "dxNumberBox",
                        label: {
                            text: "Глубина журнала, дни"
                        },
                        editorOptions: {
                            mode: "number",
                            min: 10,
                            max: 365
                        }
                    },
                    {
                        dataField: "supportBoard1",
                        editorType: "dxCheckBox",
                        label: {
                            text: "Поддержка плат версии 1"
                        }
                    },
                    {
                        dataField: "supportBoard2",
                        editorType: "dxCheckBox",
                        label: {
                            text: "Поддержка плат версии 2"
                        }
                    },
                    {
                        dataField: "supportBoard3",
                        editorType: "dxCheckBox",
                        label: {
                            text: "Поддержка плат версии 3"
                        }
                    },
                    {
                        dataField: "fileMaxUploadLenght",
                        editorType: "dxNumberBox",
                        label: {
                            text: "Максимальный размер файла для загрузки, Мб"
                        },
                        editorOptions: {
                            mode: "number",
                            min: 10,
                            max: 1000
                        }               
                    },
                    {
                        dataField: "usersMaxDownloads",
                        editorType: "dxNumberBox",
                        label: {
                            text: "Максимальное кол-во одновременных скачиваний"
                        },
                        editorOptions: {
                            mode: "number",
                            min: 5,
                            max: 1000
                        }
                    }
                ]
             }).dxForm("instance");
         });

}

function sendAppSettings() {
    const form = $("#appSettingsForm").dxForm("instance");
    const valid = form.validate();
    var appError = document.getElementById("appSettingsFormErrors");
    appError.style.display = 'none';
    if (!valid || !valid.isValid) return;

    const data = form.option('formData');

    $.postJSON(`/api/appSettings`, data, null).then(function() {
            $("#appSetingsPopup").modal("hide");
            getFiles();
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                $("#appSetingsPopup").modal("hide");
                getFiles();
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;

                appError.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                appError.style.display = "block";
            }
        });
}