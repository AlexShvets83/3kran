var timeZones = [
    {
        value: 2,
        name: "UTC+02:00 Калининград"
    },
    {
        value: 3,
        name: "UTC+03:00 Москва"
    },
    {
        value: 4,
        name: "UTC+04:00 Самара"
    },
    {
        value: 5,
        name: "UTC+05:00 Екатеринбург"
    },
    {
        value: 6,
        name: "UTC+06:00 Омск"
    },
    {
        value: 7,
        name: "UTC+07:00 Красноярск"
    },
    {
        value: 8,
        name: "UTC+08:00 Иркутск"
    },
    {
        value: 9,
        name: "UTC+09:00 Якутск"
    },
    {
        value: 10,
        name: "UTC+10:00 Владивосток"
    },
    {
        value: 11,
        name: "UTC+11:00 Магадан"
    },
    {
        value: 12,
        name: "UTC+12:00 Камчатский край"
    }
];

var currencies = [
    {
        value: "RUB",
        name: "Российский рубль"
    },
    {
        value: "KZT",
        name: "Казахстанский тенге"
    },
    {
        value: "AZN",
        name: "Азербайджанский манат"
    },
    {
        value: "UZS",
        name: "Узбекский сум"
    },
    {
        value: "BYR",
        name: "Белорусский рубль"
    }
];

function datePickerInite() {
    /*
 "locale": {
        "format": "MM/DD/YYYY",
        "separator": " - ",
        "applyLabel": "Apply",
        "cancelLabel": "Cancel",
        "fromLabel": "From",
        "toLabel": "To",
        "customRangeLabel": "Custom",
        "weekLabel": "W",
        "daysOfWeek": [
            "Su",
            "Mo",
            "Tu",
            "We",
            "Th",
            "Fr",
            "Sa"
        ],
        "monthNames": [
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        ],
        "firstDay": 1
    },
 */
    
    $('#dateRangePicker').daterangepicker({
        //singleDatePicker: true,
        showDropdowns: true,
        alwaysShowCalendars: true,
        autoUpdateInput: true,
        opens: 'left',
        ranges: {
            'Сегодня': [moment(), moment().add(1, 'days')],
            'Вчера': [moment().subtract(1, 'days'), moment()],
            'Последние 7 дней': [moment().subtract(6, 'days'), moment().add(1, 'days')],
            'Последние 30 дней': [moment().subtract(29, 'days'), moment().add(1, 'days')],
            'Этот месяц': [moment().startOf('month'), moment().endOf('month')],
            'Последний месяц': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        "locale": {
            "format": "DD.MM.YYYY",
            "separator": " - ",
            "applyLabel": "Применить",
            "cancelLabel": "Отмена",
            "fromLabel": "От",
            "toLabel": "До",
            "customRangeLabel": "Пользовательский",
            "weekLabel": "Неделя",
            "daysOfWeek": [
                "Пн.",
                "Вт.",
                "Ср.",
                "Чт.",
                "Пт.",
                "Сб.",
                "Вс."
            ],
            "monthNames": [
                "Январь",
                "Февраль",
                "Март",
                "Апрель",
                "Май",
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь",
                "Ноябрь",
                "Декабрь"
            ],
            "firstDay": 1
        },

        endDate: moment().add(1, 'days'),
        startDate: moment().subtract(6, 'days')

    });

    $('#dateRangePicker').on('apply.daterangepicker', function (ev, picker) {
        const header = document.getElementById("popupTitle").innerHTML;
        if (~header.indexOf("Продажи")) {
            getSalesByPeriod(picker.startDate, picker.endDate);
        }
        if (~header.indexOf("Инкассации")) {
            getEncashByPeriod(picker.startDate, picker.endDate);
        }
        if (~header.indexOf("Аварии")) {
            getAlertsByPeriod(picker.startDate, picker.endDate);
        }
    });
}

function popupSales() {
    const header = document.getElementById("popupTitle");
    header.innerHTML = '<i class="fa fa-shopping-cart text-warning"></i>&nbsp;&nbsp;Продажи за период';
    $('#alertsPopup').modal('toggle');

    var tooltipInstance = $("#tooltipContainer").dxTooltip({
        position: "right"
    }).dxTooltip("instance");

    $("#gridAlerts").dxDataGrid({
        //showColumnLines: true,
        scrolling: {
            mode: "infinite" // "standard" or "virtual" | "infinite"
        },
        showRowLines: true,
        keyExpr: "messageDate",
        columnAutoWidth: false,
        columnFixing: { enabled: false },
        cellHintEnabled: true,
        showBorders: true,
        wordWrapEnabled: false,
        selection: { mode: 'none' },
        height: function () {
                return window.innerHeight / 1.8;
            },
        headerFilter: {
            visible: true
        },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        loadPanel: { enabled: true },
        onCellPrepared: function (e) {
                var fieldHtml = "";
                const column = e.column;
                if (e.rowType === "data" && column.dataField === "paymentType") {
                    const sale = e.data;
                    switch (sale.paymentType) {
                    case 0:
                        fieldHtml = '<i class="fa fa-money text-success"></i>';
                        break;
                    case 1:
                        fieldHtml = '<i class="fa fa-credit-card text-warning"></i>';
                        break;
                    case -1:
                        fieldHtml = '<i class="fa fa-cloud-upload text-danger"></i>';
                        break;
                    }

                    e.cellElement.html(fieldHtml);

                    //todo finish up and uncomment 
                    //var coins = sale.coins !== null ? sale.coins.length : 0;
                    //var bills = sale.bills !== null ? sale.bills.length : 0;
                    //e.cellElement.mouseover(function (arg) {
                    //    tooltipInstance.option("contentTemplate", function (contentElement) {
                    //        contentElement.html(`<div class='tooltipContent'><div><b>Coins:</b> ${coins}</div>` + 
                    //            `<div><b>Bills:</b> ${bills}</div></div>`);
                    //    });

                    //    tooltipInstance.show(arg.target);
                    //});

                    //e.cellElement.mouseout(function (arg) {
                    //    tooltipInstance.hide();
                    //});
                }
            },
        columns: [
            {
                width: '5%',
                alignment: "center",
                dataField: "paymentType",
                allowSorting: false,
                allowHeaderFiltering: true,
                caption: "",
                //headerCellTemplate: function (header, info) {
                //    setHeader(header, info);
                //},
                headerFilter: {
                    dataSource: [
                        {
                            text: "За наличные",
                            value: ["paymentType", "=", 0],
                            template: function () {
                                    return '<i class="fa fa-money text-success" aria-hidden="true"></i>&nbsp;За наличные';
                                }
                        },
                        {
                            text: "По карте",
                            value: ["paymentType", "=", 1],
                            template: function () {
                                    return '<i class="fa fa-credit-card text-warning" aria-hidden="true"></i>&nbsp;По карте';
                                }
                        },
                        {
                            text: "Пополнение карты",
                            value: ["paymentType", "=", -1],
                            template: function () {
                                    return '<i class="fa fa-cloud-upload text-danger" aria-hidden="true"></i>&nbsp;Пополнение карты';
                                }
                        }
                    ]
                }
            },
            {
                width: '15%',
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "messageDate",
                caption: "Время",
                dataType: 'date',
                format: 'dd.MM HH:mm',
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "amountCash",
                caption: "Налич.",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "amountBill",
                caption: "Купюр",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "amountCoin",
                caption: "Монет",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "amountCard",
                caption: "Картой",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "quantity",
                caption: "Продано",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "nfcCard",
                caption: "NFC",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "coinsChange",
                caption: "Сдача",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            },
            {
                width: '10%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "rest",
                caption: "Остаток",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "messageDate",
                    summaryType: "count",
                    customizeText: function (obj) {
                            return `Зап: ${obj.value}`;
                        }
                },
                {
                    column: "amountCash",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "amountBill",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "amountCoin",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "amountCard",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "quantity",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "nfcCard",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "coinsChange",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "rest",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                }
            ]
        }
    });

    const startDate = $('#dateRangePicker').data('daterangepicker').startDate;
    const endDate = $('#dateRangePicker').data('daterangepicker').endDate;

    getSalesByPeriod(startDate, endDate);
    }

function getSalesByPeriod(startDate, endDate) {
    $.getJSON(`/api/sales/info?deviceId=${device.id}&from=${startDate.format('YYYY-MM-DD')}&to=${endDate.format('YYYY-MM-DD')}`, null)
        .then(function (data) {
                $("#gridAlerts").dxDataGrid('instance').option('dataSource', data);
            });
}

function popupEncash() {
    const header = document.getElementById("popupTitle");
    header.innerHTML = '<i class="fa fa-truck text-warning"></i>&nbsp;&nbsp;Инкассации за период';
    $('#alertsPopup').modal('toggle');
    
    $("#gridAlerts").dxDataGrid({
        scrolling: {
            mode: "infinite" // "standard" or "virtual" | "infinite"
        },
        showRowLines: true,
        keyExpr: "messageDate",
        columnAutoWidth: false,
        columnFixing: { enabled: false },
        cellHintEnabled: true,
        showBorders: true,
        wordWrapEnabled: false,
        selection: { mode: 'none' },
        height: function () {
                return window.innerHeight / 1.8;
            },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        loadPanel: { enabled: true },
        columns: [
            {
                width: '16%',
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "messageDate",
                caption: "Время",
                dataType: 'date',
                format: 'dd.MM HH:mm',
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '15%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "amount",
                caption: "Наличными",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '15%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "amountBill",
                caption: "Купюрами",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '15%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "amountCoin",
                caption: "Монетами",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: '13%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "nfcCard",
                caption: "NFC",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            },
            {
                width: '13%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "coinsChange",
                caption: "Сдача",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            },
            {
                width: '13%',
                alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "rest",
                caption: "Остаток",
                //hint: "Объем не выданных средств (значение по текущей оплате)",
                format: {
                    type: "fixedPoint",
                    precision: 2
                },
                headerCellTemplate: function (header, info) {
                    setHeader(header, info);
                }
            }
        ],
        summary: {
            totalItems: [
                {
                    column: "messageDate",
                    summaryType: "count",
                    customizeText: function (obj) {
                            return `Записей: ${obj.value}`;
                        }
                },
                {
                    column: "amount",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "amountBill",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "amountCoin",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "nfcCard",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "coinsChange",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                },
                {
                    column: "rest",
                    summaryType: "sum",
                    displayFormat: "{0}",
                    valueFormat: {
                        type: "fixedPoint",
                        precision: 2
                    }
                }
            ]
        }
    });

    const startDate = $('#dateRangePicker').data('daterangepicker').startDate;
    const endDate = $('#dateRangePicker').data('daterangepicker').endDate;

    getEncashByPeriod(startDate, endDate);
}

function getEncashByPeriod(startDate, endDate) {
    $.getJSON(`/api/encash?deviceId=${device.id}&from=${startDate.format('YYYY-MM-DD')}&to=${endDate.format('YYYY-MM-DD')}`, null)
        .then(function (data) {
                $("#gridAlerts").dxDataGrid('instance').option('dataSource', data);
            });
}

function popupAlerts() {

    const header = document.getElementById("popupTitle");
    header.innerHTML = '<i class="fa fa-exclamation-triangle text-warning"></i>&nbsp;&nbsp;Аварии за период';
    $('#alertsPopup').modal('toggle');

    $("#gridAlerts").dxDataGrid({
        scrolling: {
            mode: "infinite" // "standard" or "virtual" | "infinite"
        },
        showRowLines: true,
        keyExpr: "messageDate",
        columnAutoWidth: false,
        columnFixing: { enabled: false },
        cellHintEnabled: true,
        showBorders: true,
        wordWrapEnabled: false,
        selection: { mode: 'none' },
        height: function () {
                return window.innerHeight / 1.8;
            },
        hoverStateEnabled: true,
        rowAlternationEnabled: true,
        loadPanel: { enabled: true },
        onCellPrepared: function (options) {
                //const fieldData = options.value;
                var fieldHtml;
                const column = options.column;
                if (options.rowType === "data" && column.dataField === "codeType") {
                    const sale = options.data;
                    switch (sale.codeType) {
                    case -1:
                    case -2:
                        fieldHtml = '<i class="fa fa-exclamation-triangle text-danger"></i>';
                        break;
                    case 0:
                        fieldHtml = '<i class="fa fa-exclamation-triangle text-warning"></i>';
                        break;
                    default:
                        fieldHtml = '<i class="fa fa-check text-success"></i>';
                        break;
                    }

                    options.cellElement.html(fieldHtml);
                }
            },
        columns: [
            {
                width: "5%",
                alignment: "center",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "codeType",
                caption: "",
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                width: "15%",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "messageDate",
                caption: "Время",
                dataType: 'date',
                format: 'dd.MM HH:mm',
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    }
            },
            {
                //alignment: "right",
                allowSorting: false,
                allowHeaderFiltering: false,
                dataField: "message",
                caption: "Событие",
                headerCellTemplate: function (header, info) {
                        setHeader(header, info);
                    },
                cellTemplate: function (element, info) {
                        if (!info.data) return;
                        switch (info.data.codeType) {
                        case -1:
                        case -2:
                            element.append('<div>' + info.data.message + '</div>').addClass("text-danger");
                            break;
                        case 0:
                            element.append('<div>' + info.data.message + '</div>').addClass("text-warning");
                            break;
                        default:
                            element.append('<div>' + info.data.message + '</div>').addClass("text-success");
                            break;
                        }
                    }
            }
        ],
        summary: {
        totalItems: [
            {
                column: "messageDate",
                summaryType: "count",
                customizeText: function (obj) {
                    return `Зап.: ${obj.value}`;
                }
            }
        ]
    }
    });

    const startDate = $('#dateRangePicker').data('daterangepicker').startDate;
    const endDate = $('#dateRangePicker').data('daterangepicker').endDate;

    getAlertsByPeriod(startDate, endDate);
}

function getAlertsByPeriod(startDate, endDate) {
    $.getJSON(`/api/alerts?deviceId=${device.id}&from=${startDate.format('YYYY-MM-DD')}&to=${endDate.format('YYYY-MM-DD')}`, null)
        .then(function (data) {
                $("#gridAlerts").dxDataGrid('instance').option('dataSource', data);
            });
}

function setHeader(header, info) {
    const styles = {
        "font-size": "14px",
        "text-align": "center !important",
        "vertical-align": "middle !important",
        "padding": "0 !important"
    };

    $('<b style="color: black">')
        .html(info.column.caption)
        .css(styles)
        .appendTo(header);
}

function openDownloadFileDialog() {
        var workbook = new window.ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet('Main sheet');
        const header = document.getElementById("popupTitle");
        var hd = $.trim(header.innerText);
        DevExpress.excelExporter.exportDataGrid({
            worksheet: worksheet,
            component: $("#gridAlerts").dxDataGrid("instance"),
            customizeCell: function (options) {
                const excelCell = options;
                excelCell.font = { name: 'Arial', size: 12 };
                excelCell.alignment = { horizontal: 'left' };
            }
        }).then(function () {
            workbook.xlsx.writeBuffer().then(function (buffer) {
                saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${hd}_${device.imei}.xlsx`);
            });
        });
    }

function addDevice(dev, component) {

    var isEdit = false;
    document.getElementById("errors").style.display = "none";
    if (dev) {
        isEdit = true;
        document.getElementById("headerAddDevice").innerHTML = '<i class="fa fa-list-ul text-secondary"></i>&nbsp;&nbsp;Профиль автомата';
        const btn = document.getElementById("btnAddDevice");
        btn.className = 'btn btn-primary fa fa-save';
        btn.innerText = ' Сохранить';
        document.getElementById("popupDevId").value = dev.id;
    }
    else {
        document.getElementById("headerAddDevice").innerHTML = '<i class="fa fa-list-ul text-secondary"></i>&nbsp;&nbsp;Добавить (зарегистрировать) автомат';
        const btn = document.getElementById("btnAddDevice");
        btn.className = 'btn btn-primary fa fa-plus-square';
        btn.innerText = ' Добавить';
        document.getElementById("popupDevId").value = null;
    }

    const form = $("#deviceForm").dxForm({
        //formData: dev,
        readOnly: false,
        showColonAfterLabel: true,
        labelLocation: "top",
        items: [
            {
                dataField: "imei",
                editorType: "dxNumberBox",
                label: {
                    text: "ID автомата",
                    visible: true
                },
                editorOptions: {
                    placeholder: "Введите ID автомата (15 цифр)",
                    mode: "number",
                    value: null
                    //readOnly: isEdit
                },
                validationRules: [
                    {
                        type: "required",
                        message: "Введите ID автомата (15 цифр)"
                    },
                    {
                        type: "stringLength",
                        min: 15,
                        message: "ID автомата должен быть не менее 15 цифр"
                    },
                    {
                        type: "stringLength",
                        max: 17,
                        message: "ID автомата не должен быть длиннее 17 цифр"
                    },
                    {
                        type: "async",
                        message: "ID автомата уже зарегистрирован.",
                        validationCallback: function (params) {
                            if (isEdit) {
                                var d = $.Deferred();
                                setTimeout(function() {
                                    d.resolve(true);
                                }, 1);
                                return d.promise();
                            }
                            return checkImei(params.value);
                        }
                    }
                ]
            },
            {
                dataField: "address",
                label: {
                    text: "Адрес",
                    visible: true
                },
                editorOptions: {
                    placeholder: "Адрес автомата",
                    value: null
                },
                validationRules: [
                    {
                        type: "required",
                        message: "Нужно ввести адрес"
                    }
                ]
            },
            {
                dataField: "phone",
                editorType: "dxTextBox",
                label: {
                    text: "Телефон",
                    visible: true
                },
                editorOptions: {
                    placeholder: "Введите номер телефона",
                    value: null
                },
                validationRules: [
                    {
                        type: 'pattern',
                        pattern: '^\\d+$',
                        message: "Телефон должен состоять только из цифр"
                    }
                ]
            },
            {
                dataField: "timeZone",
                editorType: "dxSelectBox",
                label: {
                    text: "Часовой пояс",
                    visible: true
                },
                editorOptions: {
                    dataSource: timeZones,
                    valueExpr: "value",
                    displayExpr: "name",
                    value: 2
                }
            },
            {
                dataField: "currency",
                editorType: "dxSelectBox",
                label: {
                    text: "Валюта",
                    visible: true
                },
                editorOptions: {
                    dataSource: currencies,
                    valueExpr: "value",
                    displayExpr: "name",
                    value: "RUB"
                }
            }
        ]
        //minColWidth: 300,
        //colCount: 2
    }).dxForm("instance");

    if (isEdit) {
        let editor = form.getEditor("imei");
        editor.option("value", Number(dev.imei));

        editor = form.getEditor("address");

        editor.option("value", dev.address ? dev.address : null);

        editor = form.getEditor("phone");
        editor.option("value", dev.phone);

        editor = form.getEditor("timeZone");
        editor.option("value", dev.timeZone);

        editor = form.getEditor("currency");
        editor.option("value", dev.currency);
        getInfos(dev.id, component);
    }
}

function postDevice() {
    const dfd = new $.Deferred();
    document.getElementById("errors").style.display = "none";
    const valid = $('#deviceForm').dxForm('instance').validate();
    if (valid.isValid === false) dfd.reject(false);;

    const data = $('#deviceForm').dxForm('instance').option('formData');
    data.imei = `${data.imei}`;
    data.phone = `${data.phone}`;

    const dev = {
        imei: `${data.imei}`,
        address: data.address,
        phone: data.phone,
        timeZone: data.timeZone,
        currency: data.currency,
        ownerId: null
    }

    const id = document.getElementById("popupDevId").value;
    var request;
    if (id) {
        request = $.putJSON(`/api/device/${id}`, dev, null);
        device.address = data.address;
        device.phone = data.phone;
        device.timeZone = data.timeZone;
        device.currency = data.currency;
    } else {
        request = $.postJSON('/api/device', dev, null);
    }
   
    request.then(function() {
            $('#popupDevice').modal('hide');
            dfd.resolve(true);
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                $('#popupDevice').modal('hide');
                dfd.resolve(true);
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;
                document.getElementById("errors").innerHTML = message;
                document.getElementById("errors").style.display = "block";
                dfd.reject(false);
            }
        });
    return dfd.promise();
}

function devSettings(dev) {

    var isInit = true;
    const form = $("#deviceSettingsForm").dxForm({
        //formData: dev,
        readOnly: false,
        showColonAfterLabel: true,
        labelLocation: "left", // or "left" | "right" "top"
        visible: false,
        items: [
            {
                dataField: "pulsesPerLitre",
                editorType: "dxNumberBox",
                label: {
                    text: "Количество импульсов на литр",
                    visible: true
                },
                editorOptions: {
                    mode: "number",
                    value: null,
                    format: "####",
                    min: 0,
                    max: 9999,
                    onValueChanged: function (data) {
                        if (!data.previousValue) return;
                        if (data.value >= 0 && data.value <= 9999) {
                            postSettings('pulsesPerLitre', data.value);
                        }
                    }
                }
            },
            {
                dataField: "pricePerLitre",
                editorType: "dxNumberBox",
                label: {
                    text: "Цена за литр",
                    visible: true
                },
                editorOptions: {
                    mode: "number",
                    value: null,
                    format: "#####0.0##",
                    min: 0,
                    max: 100000,
                    onValueChanged: function (data) {
                        if (!data.previousValue) return;
                        if (data.value >= 0 && data.value <= 100000) {
                            postSettings('pricePerLitre', data.value);
                        }
                    }
                }
            },
            {
                dataField: "priceCard",
                editorType: "dxNumberBox",
                label: {
                    text: "Цена по карте",
                    visible: true
                },
                editorOptions: {
                    mode: "number",
                    value: null,
                    format: "#####0.0##",
                    min: 0,
                    max: 100000,
                    onValueChanged: function (data) {
                        if (!data.previousValue) return;
                        if (data.value >= 0 && data.value <= 100000) {
                            postSettings('priceCard', data.value);
                        }
                    }
                }
            },
            {
                dataField: "pulseValueCoin",
                editorType: "dxNumberBox",
                label: {
                    text: "Цена импульса монетоприемника",
                    visible: true
                },
                editorOptions: {
                    mode: "number",
                    value: null,
                    format: "###.0##",
                    //value: 20,
                    min: 0,
                    max: 100,
                    onValueChanged: function (data) {
                        if (!data.previousValue) return;
                        if (data.value >= 0 && data.value <= 100) {
                            postSettings('pulseValueCoin', data.value);
                        }
                    }
                }
            },
            {
                dataField: "pulseValueBill",
                editorType: "dxNumberBox",
                label: {
                    text: "Цена импульса купюроприемника",
                    visible: true
                },
                editorOptions: {
                    mode: "number",
                    value: null,
                    format: "####.0##",
                    //value: 20,
                    min: 0,
                    max: 1000,
                    onValueChanged: function (data) {
                        if (!data.previousValue) return;
                        if (data.value >= 0 && data.value <= 1000) {
                            postSettings('pulseValueBill', data.value);
                        }
                    }
                }
            },
            {
                dataField: "therm",
                editorType: "dxNumberBox",
                label: {
                    text: "Заданная температура",
                    visible: true
                },
                editorOptions: {
                    mode: "number",
                    value: null,
                    format: "###.0##",
                    min: -10,
                    max: 30,
                    onValueChanged: function (data) {
                        if (!data.previousValue) return;
                        if (data.value >= -10 && data.value <= 30) {
                            postSettings('therm', data.value);
                        }
                    }
                }
            },
            {
                dataField: "logo",
                editorType: "dxSelectBox",
                label: {
                    text: "Приветственное сообщение",
                    visible: true
                },
                editorOptions: {
                    dataSource: [ 
                        { value: 0, name: 'Третий кран' },
                        { value: 1, name: 'Добро пожаловать'},
                        { value: 2, name: 'Продажа воды'}
                    ],
                    valueExpr: "value",
                    displayExpr: "name",
                    value: null,
                    onValueChanged: function (data) {
                        if (data.previousValue == null) return;
                        
                        postSettings('logo', data.value);
                    }
                }
            },
            {
                dataField: "date",
                editorType: "dxDateBox",
                label: {
                    text: "Дата смены фильтров",
                    visible: true
                },
                editorOptions: {
                    displayFormat: "dd.MM.yyyy",
                    type: "date",
                    applyValueMode: "useButtons",
                    value: null,
                    //max: new Date(),
                    min: new Date(2000, 1, 1),
                    onValueChanged: function(data) {
                        if (data.previousValue == null) return;
                        
                        postSettings('date', data.value);
                    }
                }
            },
            {
                dataField: "phone",
                editorType: "dxTextBox",
                label: {
                    text: "Телефон сервисной поддержки",
                    visible: true
                },
                editorOptions: {
                    value: null,
                    onValueChanged: function(data) {
                        if (data.previousValue == null) return;
                        if (data.value) {
                            if (!$.isNumeric(data.value)) return;
                        }

                        postSettings('phone', data.value);
                    }
                },
                validationRules: [
                    {
                        type: 'pattern',
                        pattern: '^\\d+$',
                        message: "Телефон должен состоять только из цифр"
                    }
                ]
            },
            {
                dataField: "maintain",
                editorType: "dxSwitch",
                label: {
                    //location: "left",
                    //alignment: "right", // or "left" | "center"
                    text: "Режим обслуживания",
                    visible: true
                },
                editorOptions: {
                    switchedOffText:"Выключен",
                    switchedOnText:"Включен",
                    height: 50,
                    width: 100,
                    elementAttr: {
                        //style: "color:red;font-size:40px;"
                        class: "dx-field-item-label"
                    },
                    value: null,
                    onValueChanged: function (data) {
                        //if (data.previousValue == null) return;
                        if (isInit) {
                            isInit = false;
                            return;
                        }
                        const val = data.value ? 1 : 0;
                        postSettings('maintain', val);
                    }
                }
            }
        ]
        //Телефон сервисной поддержки
        //minColWidth: 300,
        //colCount: 2
    }).dxForm("instance");

    $.getJSON("/api/device/settings/" + dev.id, null)
        .then(function(data) {
            if (data) {
                form.option("visible", true);
                let editor = form.getEditor("pulsesPerLitre");
                editor.option("value", Number(data.pulsesPerLitre));

                editor = form.getEditor("pricePerLitre");
                editor.option("value", Number(data.pricePerLitre));

                editor = form.getEditor("priceCard");
                editor.option("value", Number(data.priceCard));

                editor = form.getEditor("pulseValueCoin");
                editor.option("value", Number(data.pulseValueCoin));

                editor = form.getEditor("pulseValueBill");
                editor.option("value", Number(data.pulseValueBill));

                editor = form.getEditor("therm");
                editor.option("value", Number(data.therm));
                
                editor = form.getEditor("logo");
                editor.option("value", Number(data.logo));

                editor = form.getEditor("date");
                const date = new Date(data.date*1000);
                editor.option("value", Number(date));
                
                editor = form.getEditor("phone");
                editor.option("value", data.phone);

                editor = form.getEditor("maintain");
                const mn = data.maintain === 0 ? false : true;
                if (!mn) isInit = false;
                editor.option("value", mn);
            } else {
                form.option("visible", false);
            }
        });
}

var checkImei = function(value) {
    const get = $.get(`/api/validation/imei/${value}`);
    return get.promise();
}

function postSettings(pName, pValue) {

    const payload = { name: pName, value: pValue };
    $.putJSON(`/api/device/settings/${device.imei}`, payload, null).then(function() {
        return;
    });
}

function getInfos(id, componentId) {
         $.getJSON(`/api/device/info/${id}`, null)
             .then(function (data) {
                 if (data == null || data.count === 0) return;

                 var metrics = [];
                 if (device.lastCleanerStatus) {
                     const clV = device.lastCleanerStatus.tds.toFixed(0);
                     let cl = 'badge-success';
                     if (clV > 50.0) { cl = 'badge-warning'; }
                     if (clV < 0.0 || clV > 100.0) {cl = 'badge-danger';}
                     metrics.push(`<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-tint"></i> ${clV}<br> <small>TDS, ppm</small> </span> </info-badge>`);
                 }
                 if (device.lastStatus) {
                     if (device.lastStatus.temperature) {
                         const stV = device.lastStatus.temperature;
                         let clT = 'badge-success';
                         if (stV < 5.0 || stV > 40.0) { clT = 'badge-warning'; }
                         if (stV < 0.0 || stV > 50.0) {clT = 'badge-danger';}
                         metrics.push(`<info-badge><span class="badge mr-1 mb-1 ${clT}"> <i class="fa pr-1 fa-thermometer"></i> ${stV}<br> <small>Температура, &deg;C</small> </span> </info-badge>`);
                     }
                 }

                 data.forEach(function (h) {
                     //todo add toltip
                     const nameM = h["name"];
                     const valueM = h["value"];
                     var i = '';
                     var cl = 'badge-success';
                     switch (nameM) {
                     case 'signalStrength':
                         if (valueM < 15.0) { cl = 'badge-danger';}

                         if (valueM < 20.0) { cl = 'badge-warning';}

                         i = `<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-wifi"></i> ${valueM}<br> <small>Cигнал</small> </span> </info-badge>`;
                         //'fa-wifi
                         break;
                     case 'simBalance':
                         if (valueM < 20.0) cl = 'badge-danger';
                         i = `<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-wifi"></i> ${valueM}<br> <small>Баланс SIM</small> </span> </info-badge>`;
                         //'fa-wifi';
                         break;
                     case 'energyT1':
                         if (valueM < 0.0) cl = 'badge-danger';
                         i = `<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-plug"></i> ${valueM}<br> <small>Тариф 1, КВт&middot;ч</small> </span> </info-badge>`;
                         //'fa-plug'
                         break;
                     case 'energyT2':
                         if (valueM < 0.0) cl = 'badge-danger';
                         i = `<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-plug"></i> ${valueM}<br> <small>Тариф 2, КВт&middot;ч</small> </span> </info-badge>`;
                         //'fa-plug';
                         break;
                     case 'waterInput':
                         if (valueM < 0.0) cl = 'badge-danger';
                         i = `<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-tint"></i> ${valueM}<br> <small>Водомер, м&sup3;</small> </span> </info-badge>`;
                         //'fa-tint';
                         break;
                     case 'tds':
                         //0.0, 0.0, 100.0, 50.0
                         if (valueM > 50.0) { cl = 'badge-warning'; }
                         if (valueM < 0.0 || valueM > 100.0) {cl = 'badge-danger';}
                         i = `<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-tint"></i> ${valueM}<br> <small>TDS, ppm</small> </span> </info-badge>`;
                         //'fa-tint'
                         break;
                     case 'temperature':
                         if (valueM < 5.0 || valueM > 40.0) { cl = 'badge-warning'; }
                         if (valueM < 0.0 || valueM > 50.0) {cl = 'badge-danger';}
                         //0.0, 5.0, 50.0, 40.0
                         i = `<info-badge><span class="badge mr-1 mb-1 ${cl}"> <i class="fa pr-1 fa-thermometer"></i> ${valueM}<br> <small>Температура, &deg;C</small> </span> </info-badge>`;
                         //'fa-thermometer';
                         break;
                     }
                     //var i = `<div><i class="fa fa-truck text-warning"></i>&nbsp;&nbsp;${dd}&nbsp;&ndash;&nbsp;${h["amount"]}${getCurrencySign(device)}</div>`;
                     
                     metrics.push(i);
                 });

                 const devM = document.getElementById(componentId);
                 devM.innerHTML = metrics.join("");
             });
}

function userLogCreate() {
    $.getJSON('/api/log/getLog', null)
        .then(function (data) {
            $("#userLogTable").dxDataGrid({
                dataSource: data,
            showColumnLines: true,
            //focusedRowEnabled: true,
            showRowLines: true,
            keyExpr: "id",
            columnAutoWidth: true,
            columnFixing: { enabled: false },
            cellHintEnabled: true,
            paging: { pageSize: 10 },
            pager: {
                showInfo: true,
                infoText: 'Страница {0} из {1} ({2} действий)'
            },
            showBorders: true,
            wordWrapEnabled: false,
            selection: { mode: 'none' },
                //width: 500,
            //height: 521,
            hoverStateEnabled: true,
            loadPanel: { enabled: false },
            searchPanel: {
                visible: true,
                highlightCaseSensitive: true,
                placeholder: "Начните вводить email, моб. телефон или IMEI...",
                width: '100%'
            },
            columns: [
                {
                    dataField: "email",
                    visible: false
                },
                {
                    dataField: "phone",
                    visible: false
                },
                {
                    dataField: "imei",
                    visible: false
                },
                {
                    width: 130,
                    dataField: "logDate",
                    allowSorting: false,
                    caption: "Дата",
                    dataType: "date",
                    //format: {type: "shortDateShortTime"},
                    format: "dd.MM.yyyy HH:mm",
                    headerCellTemplate: function(header, info) {
                        setHeader(header, info);
                    }
                },
                {
                    allowSorting: false,
                    dataField: "message",
                    caption: "Действие",
                    dataType: "string",
                    headerCellTemplate: function(header, info) {
                        setHeader(header, info);
                    }
                }
            ]
        });

        });
}
