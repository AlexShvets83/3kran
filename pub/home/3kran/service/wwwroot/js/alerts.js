function alertsInit() {
    const success = document.getElementById("manageAlertsSuccess");
    success.style.display = "none";
    const error = document.getElementById("manageAlertsError");
    error.style.display = "none";

    const request = $.getJSON("/api/account", null);
    request.done(function(data) {
        sessionStorage.setItem(accountKey, JSON.stringify(data));
        var edm = [{ email: data.email }];

        data.infoEmails.forEach(function(i) {
            edm.push({ email: i });
        });

        $("#emailTable").dxDataGrid({
        dataSource: edm,
        editing: {
            mode: "popup",
            texts: {
                confirmDeleteMessage: ""
            }
        },
        scrolling: {
            mode: "infinite" // "standard" or "virtual" | "infinite"
        },
        showRowLines: true,
        columnAutoWidth: false,
        columnFixing: { enabled: false },
        cellHintEnabled: true,
        showBorders: true,
        wordWrapEnabled: false,
        selection: { mode: "none" },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        columns: [
            {  
                width: "5%",
                allowSorting: false,
                allowHeaderFiltering: false,
                caption: "#",  
                cellTemplate: function(cellElement, cellInfo) {
                    cellElement.text(cellInfo.row.rowIndex + 1);
                },
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            },  
            {
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "email",
                caption: "Email",
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            },
{
                    width: 30,
                    minWidth: 30,
                    headerCellTemplate: function(header, info) {
                        setHeader(header, info);
                    },
                    type: "buttons",
                    buttons: [
                        {
                            hint: "Удалить",
                            cssClass: "fa fa-window-close text-danger",
                            visible: function(e) {
                                return e.row.data.email !== data.email;
                            },
                            onClick: function(e) {
                                const grid = $("#emailTable").dxDataGrid("instance");
                                grid.deleteRow(e.row.rowIndex);
                                grid.deselectAll();
                            }
                        }
                    ]
                }
        ]
        
    });
        
        var txt = $("#emailsAdd").dxTextBox({
            width: "100%",
            mode: "email",
            value: null,
            buttons: [{
                id: "emailsAddBtn",
                name: "emailsAddBtn",
                location: "after",
                options: {
                    icon: "add",
                    type: "default",
                    onClick: function() {

                        const valid = $("#emailsAdd").dxValidator("instance").validate().isValid;
                        if (!valid) return;
                        const grid = $("#emailTable").dxDataGrid("instance");

                        //проверить на присутствие и очистить тексбокс
                        var email = $("#emailsAdd").dxTextBox("instance").option("value");
                        var pr = false;
                        edm.forEach(function(e) {
                            const ml = e["email"];
                            if (ml === email) {
                                pr = true;
                                return;
                            }

                        });

                        if (pr) return;
                        if (edm.length === 4) return;
                        edm.push({ email: email });
                        grid.option("dataSource", edm);
                        $("#emailsAdd").dxTextBox("instance").option("value", null);
                    }
                }
            }]
        }).dxValidator({
            validationRules: [
                {
                    type: "required",
                    message: "Введите email."
                },
                {
                type: "email",
                message: "Неверный email."
            }]
        });
        
        $("#alertsCheckTbl").dxDataGrid({
            dataSource: data.alerts,
            editing: {
                mode: "cell",
                allowUpdating: true,
                allowAdding: false,
                allowDeleting: false
            }, 
            showRowLines: true,
            columnAutoWidth: false,
            columnFixing: { enabled: false },
            cellHintEnabled: true,
            showBorders: true,
            wordWrapEnabled: false,
            selection: { mode: "none" },
            hoverStateEnabled: true,
            rowAlternationEnabled: true,
            columns:[
                {
                    width: "5%",
                    allowSorting: false,
                    allowHeaderFiltering: false,
                    caption: "#",  
                    cellTemplate: function(cellElement, cellInfo) {
                        cellElement.text(cellInfo.row.rowIndex + 1);
                    },
                    headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
                },
                {
                    allowSorting: false,
                    allowHeaderFiltering: false,
                    caption: "Событие",
                    dataField: "description",
                    headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
                },
                {
                    width: "20%",
                    allowSorting: false,
                    allowHeaderFiltering: false,
                    dataField: "active",
                    caption: "Выбрать",
                    allowEditing: true,
                    headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
                }
            ]
   
        });
        

        var df = $("#emailsAdd").dxTextBox("instance");
        df.option("isValid", true);
    });

    //if (txt !== null) txt.option("isValid", true);
//} 
    //var df = $("#emailsAdd").dxTextBox("instance");
    //df.option("isValid", true);
}

function alertSave() {
    var success = document.getElementById("manageAlertsSuccess");
    success.style.display = "none";
    var error = document.getElementById("manageAlertsError");
    error.style.display = "none";

    const accStr = sessionStorage.getItem(accountKey);
    if (!accStr) {
        window.location.replace("/Account/Login");
    }
    var acc = JSON.parse(accStr);

    var emails = [];

    const emlD = $("#emailTable").dxDataGrid("instance").option("dataSource");
    emlD.forEach(function(i) {
        if (i.email !== acc.email) emails.push(i["email"]);
    });

    const alerts = $("#alertsCheckTbl").dxDataGrid("instance").option("dataSource");

    const data = { infoEmails: emails, userAlerts: alerts };
    $.putJSON(`/api/Account/setInfos`, data, null).then(
        function() {
            success.style.display = "block";
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                success.style.display = "block";
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;
                error.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                error.style.display = "block";
            }
        }
    );
}