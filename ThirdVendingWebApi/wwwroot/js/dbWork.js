var clearPeriod;
function dbWorkInit() {
    $("#dbRecoveryList").dxList({
        dataSource: [],
        //height: 400,
        allowItemDeleting: false,
        itemDeleteMode: "toggle",
        showSelectionControls: true,
        selectionMode: "single"
        //onSelectionChanged: updateSelectedItems,
        //onItemDeleted: updateSelectedItems
    }).dxList("instance");

    getRestoreFiles();
}

function getRestoreFiles() {
    var dbErrors = document.getElementById("dbErrors");
    dbErrors.style.display = "none";
    $.getJSON("/api/dbWork", null).then(
        function (data) {
            $("#dbRecoveryList").dxList("instance").option("dataSource", data);
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                $("#dbRecoveryList").dxList("instance").option("dataSource", data);
            } 
            else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;
                dbErrors.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                dbErrors.style.display = "block";
            }
        }
    );
}

function dbBackup() {
    var dbErrors = document.getElementById("dbErrors");
    dbErrors.style.display = "none";
    $.postJSON(`/api/dbWork`, null, null).then(
        function() {
            getRestoreFiles();
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                getRestoreFiles();
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;
                dbErrors.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                dbErrors.style.display = "block";
            }
        }
    );
}

function initDbRestore() {
    var cap = document.getElementById("restoreFileCaption");
    var rf = $("#dbRecoveryList").dxList("instance").option("selectedItems");
    $("#restoreDbBtn").prop("disabled", true);
    $("#chkRestore").prop("checked", false);
    //document.getElementById("restoreDbBtn").disabled = true;
    //document.getElementById("chkRestore").checked = false;
    $("#chkRestore").change(function () {
        document.getElementById("restoreDbBtn").disabled = !$(this).prop("checked");
    });
    //todo сообщение выбрать файл
    if (rf.length === 0) {
        $("#chkRestore").prop("disabled", true);
        cap.innerText = "Выберите файл восстановления!";
        //$("#restoreDbPopup").modal("hide");
        return;
    }
    $("#chkRestore").prop("disabled", false);
    cap.innerText = `Вы собираетесь восстановить БД из ${rf[0]}. Все текущие данные будут замещены.`;

}

function dbRestore() {
    var dbErrors = document.getElementById("restoreDbError");
    dbErrors.style.display = "none";
    var rf = $("#dbRecoveryList").dxList("instance").option("selectedItems");
    //todo сообщение выбрать файл
    if (rf.length === 0) return;

    $.putJSON(`/api/dbWork`, rf[0], null).then(
        function() {
            $("#restoreDbPopup").modal("hide");
            //getRestoreFiles();
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                $("#restoreDbPopup").modal("hide");
                //getRestoreFiles();
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;
                dbErrors.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                dbErrors.style.display = "block";
            }
        }
    );
}

function initDbClear() {
    clearPeriod = null;
    var dbErrors = document.getElementById("clearDbErrors");
    dbErrors.style.display = "none";
    $("#clearDbBtn").prop("disabled", true);
    $("#chkClear").prop("checked", false);
    $("#chkClear").change(function () {
        document.getElementById("clearDbBtn").disabled = !$(this).prop("checked");
    });
    $.getJSON("/api/dbWork/getDate", null).then(
        function (data) {
            document.getElementById("clearDbDatapicker").style.display = "block";
            $("#chkClear").prop("disabled", false);
            $("#clearDbDatapicker").daterangepicker({
                //singleDatePicker: true,
                showDropdowns: true,
                alwaysShowCalendars: true,
                autoUpdateInput: true,
                opens: "left",
                //ranges: {
                //    'Сегодня': [moment(), moment().add(1, 'days')],
                //    'Вчера': [moment().subtract(1, 'days'), moment()],
                //    'Последние 7 дней': [moment().subtract(6, 'days'), moment().add(1, 'days')],
                //    'Последние 30 дней': [moment().subtract(29, 'days'), moment().add(1, 'days')],
                //    'Этот месяц': [moment().startOf('month'), moment().endOf('month')],
                //    'Последний месяц': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                //},
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


                startDate: moment(data.startDate),
                endDate: moment(data.endDate)

            });

            clearPeriod = {startDate: moment(data.startDate), endDate: moment(data.endDate)};

            $("#clearDbDatapicker").on("apply.daterangepicker", function (ev, picker) {
                clearPeriod = {startDate: picker.startDate, endDate: picker.endDate};
            });
        },
        function(res) {
            document.getElementById("clearDbDatapicker").style.display = "none";
            const message = res.responseText ? res.responseText : `Error${res.status}`;
            dbErrors.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
            dbErrors.style.display = "block";
            $("#chkClear").prop("disabled", true);
        }
    );



}

function dbClear() {
    if (!clearPeriod || !clearPeriod.startDate || !clearPeriod.endDate) return;

    var clearDbErrors = document.getElementById("clearDbErrors");
    clearDbErrors.style.display = "none";
    var dbErrors  = document.getElementById("dbErrors");
    dbErrors.style.display = "none";
    $.deleteObj(`/api/dbWork?startDate=${clearPeriod.startDate.format('YYYY-MM-DD')}&endDate=${clearPeriod.endDate.format('YYYY-MM-DD')}`).then(function(data) {
            $("#clearDbPopup").modal("hide");
            dbErrors.innerHTML = `<i class="fa fa-succeed" aria-hidden="true"></i>&nbsp;&nbsp;Удалено ${data} записей!`;
            dbErrors.style.display = "block";
        },
        function(res) {
            if (res.readyState === 4 && res.status === 200) {
                $("#clearDbPopup").modal("hide");
            } else {
                const message = res.responseText ? res.responseText : `Error${res.status}`;

                clearDbErrors.innerHTML = `<i class="fa fa-warning" aria-hidden="true"></i>&nbsp;&nbsp;${message}`;
                clearDbErrors.style.display = "block";
            }
        });
}