var mailDestList = [];

function initEmailSender() {
    $("#mailTheme").dxTextBox({
        placeholder: "Тема сообщения...",
        onValueChanged: function() {
            checkMail();
        }
    });
    //    .dxValidator({
    //    validationRules: [
    //        {
    //            type: "required",
    //            message: "Введите тему сообщения."
    //        }, 
    //        {
    //            type: "stringLength",
    //            min: 3,
    //            message: "Тема сообщения должна быть не менее 3 символов."
    //        }
    //    ]
    //});

    $("#mailBody").dxTextArea({
        placeholder: "Текст сообщения...",
        height: function () {
            return (window.innerHeight / 1.4) - 77;// - 46;
        },
        onValueChanged: function() {
            checkMail();
        },
        onInput: function(e) {
            var val = e.element.find(":input:not([type=hidden])")[0].value;
            var isBody = true;
            if (!val.trim()) isBody = false;
            checkMail(isBody);
        }
    }).dxTextArea("instance");
    

    $("#mailDestTable").dxDataGrid({
        dataSource: mailDestList,
        showRowLines: true,
        keyExpr: "id",
        columnAutoWidth: false,
        columnFixing: { enabled: false },
        cellHintEnabled: true,
        showBorders: true,
        wordWrapEnabled: false,
        selection: { mode: 'none' },
        height: function () {
                return (window.innerHeight / 1.4) - 46;
            },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        loadPanel: { enabled: true },
        paging: { pageSize: 10 },
        pager: {
            infoText: 'Страница {0} из {1} ({2} пользователей)',
            showInfo: true,
            visible: "always"
        },
        columns: [
            {
                fixed: false,
                width: 30,
                minWidth: 30,
                type: "buttons",
                cssClass: "vert-btn-align",
                buttons: [
                    {
                        hint: "Удалить из списка адресатов",
                        cssClass: "btn btn-secondary btn-sm btn-font text-white fa fa-minus",
                        onClick: function (e) {
                            var row = e.row.data;

                            mailDestList = jQuery.grep(mailDestList, function(value) {
                                return value !== row;
                            });

                            chekMailList();
                        }
                    }
                ]
                },
            {
                dataField: "lastName",
                allowSorting: true,
                caption: "Фамилия/email",
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    },
                cellTemplate: function (element, info) {
                    element.append('<div>' + info.value + '</div>');
                    element.append('<div>' + info.data.email + '</div>');
                }
            },
            {
                dataField: "email",
                visible: false
            },
            {
                allowSorting: true,
                dataField: "role",
                caption: "Роль",
                cssClass: "vert-align",
                dataType: "string",
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                },
                cellTemplate: function (element, info) {
                    element.append(`<div>${getNornRole(info.value)}</div>`);
                    if (info.data.ownerEmeil) {
                        element.append(`<div>${info.data.ownerEmeil}</div>`);
                    }
                },
                headerFilter: {
                    dataSource: [
                        {
                            text: "Техник",
                            value: ["role", "=", "technician"]
                        },
                        {
                            text: "Владелец",
                            value: ["role", "=", "owner"]
                        },
                        {
                            text: "Адм. дилера",
                            value: ["role", "=", "dealer_admin"]
                        },
                        {
                            text: "Дилер",
                            value: ["role", "=", "dealer"]
                        },
                        {
                            text: "Админ",
                            value: ["role", "=", "admin"]
                        }
                    ]
                }
            }
        ]
    });

    $("#mailUsersTable").dxDataGrid({
        showRowLines: true,
        keyExpr: "id",
        columnAutoWidth: false,
        columnFixing: { enabled: false },
        cellHintEnabled: true,
        showBorders: true,
        wordWrapEnabled: false,
        selection: { mode: 'none' },
        height: function () {
                return window.innerHeight / 1.4;
            },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        loadPanel: { enabled: true },
        searchPanel: {
            visible: true,
            highlightCaseSensitive: true,
            placeholder: "Поиск пользователя"
        },
        paging: { pageSize: 10 },
        pager: {
            infoText: 'Страница {0} из {1} ({2} пользователей)',
            showInfo: true,
            visible: "always"
        },
        columns: [
            {
                fixed: false,
                width: 32,
                minWidth: 30,
                type: "buttons",
                cssClass: "vert-btn-align",
                buttons: [
                    {
                        hint: "Добавить в список адресатов",
                        cssClass: "btn btn-success btn-sm btn-font text-white fa fa-plus",
                        onClick: function (e) {
                            var row = e.row.data;
                            const index = mailDestList.findIndex(x => x.id === row.id);
                            if (index >= 0) return;

                            mailDestList.push(row);
                            chekMailList();
                        }
                    }
                ]
                },
            {
                dataField: "lastName",
                allowSorting: true,
                caption: "Фамилия/email",
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    },
                cellTemplate: function (element, info) {
                    element.append('<div>' + info.value + '</div>');
                    element.append('<div>' + info.data.email + '</div>');
                }
            },
            {
                dataField: "email",
                visible: false
            },
            {
                allowSorting: true,
                dataField: "role",
                caption: "Роль",
                cssClass: "vert-align",
                dataType: "string",
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                },
                cellTemplate: function (element, info) {
                    element.append(`<div>${getNornRole(info.value)}</div>`);
                    if (info.data.ownerEmeil) {
                        element.append(`<div>${info.data.ownerEmeil}</div>`);
                    }
                },
                headerFilter: {
                    dataSource: [
                        {
                            text: "Техник",
                            value: ["role", "=", "technician"]
                        },
                        {
                            text: "Владелец",
                            value: ["role", "=", "owner"]
                        },
                        {
                            text: "Адм. дилера",
                            value: ["role", "=", "dealer_admin"]
                        },
                        {
                            text: "Дилер",
                            value: ["role", "=", "dealer"]
                        },
                        {
                            text: "Админ",
                            value: ["role", "=", "admin"]
                        }
                    ]
                }
            }
        ]
    });

    $("#mailTheme").dxTextBox("instance").option("isValid", true);
    $("#mailDestTable").dxDataGrid("instance").option("dataSource", mailDestList);

    $.getJSON("/api/users/", null)
        .then(function (data) {
                $("#mailUsersTable").dxDataGrid('instance').option('dataSource', data);
            },
            function (res) {
                if (res.status === 401 || res.status === 403) logout();
            });
}

//function name(tbName) {
//    var dataGrid = $("#tbName").dxDataGrid("instance");
//    dataGrid.refresh()
//        .done(function() {
//            // ...
//        })
//        .fail(function(error) {
//            // ...
//        });
//}

function clearMailAll() {
    clearMailList();
    $("#mailTheme").dxTextBox("instance").option("value", "");
    $("#mailBody").dxTextArea("instance").option("value", "");
    checkMail();
}
function clearMailList() {
    mailDestList = [];
    chekMailList();
}


function chekMailList() {
    $("#mailDestTable").dxDataGrid("instance").option("dataSource", mailDestList);
    const btn = document.getElementById("mailClearBtn");
    if (mailDestList.length > 0) {
        btn.disabled = false;
        btn.innerText = ` Очистить список (${mailDestList.length})`;
    } else {
        btn.disabled = true;
        btn.innerText = ` Очистить список`;
    }
    checkMail();
}

function checkMail(isBd) {
    var isTheme = true;
    const theme = $("#mailTheme").dxTextBox("instance").option("value");
    var isBody = true;
    const body = $("#mailBody").dxTextArea("instance").option("value");
    if (!theme.trim()) {
        // is empty or whitespace
        isTheme = false;
    }
    if (!isBd && !body.trim()) isBody = false;

    document.getElementById("mailSendBtn").disabled = mailDestList.length === 0 || !isTheme || !isBody;
}

function sendMail() {
    var isTheme = true;
    var isBody = true;
    const theme = $("#mailTheme").dxTextBox("instance").option("value");
    const body = $("#mailBody").dxTextArea("instance").option("value");
    if (!theme.trim()) isTheme = false;
    if (!body.trim()) isBody = false;
    if (mailDestList === 0 || !isTheme || !isBody) {
        document.getElementById("mailSendBtn").disabled = true;
        return;
    }

    var emails = [];
    mailDestList.forEach(function(itm) {
        emails.push(itm.email);
    });

    const emailModel = { emailTheme: theme, emailBody: body, addressees: emails };
    $.postJSON(`/api/Email/sendMail`, emailModel, null).then(function() {
        clearMailAll();
        $('#emailSend').modal('hide');
        return;
    });

    clearMailAll();
    $('#emailSend').modal('hide');
}