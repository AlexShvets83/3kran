function sendInvite() {
        var usrError = document.getElementById("inviteError");
        usrError.style.display = 'none';

        const form = $("#inviteForm").dxForm("instance");
        const valid = form.validate();
        if (!valid || !valid.isValid) return;

        const data = form.option('formData');
        var loadPanel = $("#loadpanel").dxLoadPanel({
            shadingColor: "rgba(0,0,0,0.4)",
            position: { of: "#employee" },
            visible: false,
            showIndicator: true,
            showPane: true,
            shading: true,
            closeOnOutsideClick: false
        }).dxLoadPanel("instance");

        loadPanel.show();
        $.postJSON('/api/account/invite', data, null).then(function() {
                loadPanel.hide();
                $('#invitePopup').modal('hide');

            },
            function(res) {
                if (res.readyState === 4 && res.status === 200) {
                    loadPanel.hide();
                    $('#invitePopup').modal('hide');
                } else {
                    loadPanel.hide();
                    const message = res.responseText ? res.responseText : `Error${res.status}`;
                    document.getElementById("inviteError").innerHTML = message;
                    document.getElementById("inviteError").style.display = "block";
                }
            });

}


function openInvitePopup() {
    var ownerEditor;
    const form = $("#inviteForm").dxForm({
        readOnly: false,
        showColonAfterLabel: true,
        labelLocation: "left",
        items: [
            {
                dataField: "email",
                label: {
                    text: "Email",
                    visible: true
                },
                validationRules: [
                    {
                        type: "required",
                        message: "Введите email."
                    }, {
                        type: "email",
                        message: "Неверный email."
                    } 
                    ,{
                        type: "async",
                        message: "Email уже зарегистрирован.",
                        validationCallback: function (params) {
                            return checkEmail(params.value);
                        }
                    }
                ]
            },
            {
                dataField: "role",
                editorType: "dxSelectBox",
                label: {
                    text: "Роль"
                },
                editorOptions: {
                    dataSource: roles,
                    valueExpr: "name",
                    displayExpr: "view",
                    //value: "technician",
                    onValueChanged: function (data) {
                        var role = data.value;
                        var dEditor = form.getEditor("ownerId"); 
                        switch (role) {
                        case "technician":
                            dEditor.option("disabled", false);
                            form.itemOption("ownerId", "label", { text: "Владелец"});
                            $.getJSON("/api/users/getAllOwners", null)
                                .then(function (data) {
                                        ownerEditor.option('dataSource', data);
                                        //if (data && data.length > 0) {
                                        //    dEditor.option('value', data[0]);
                                        //}
                                    },
                                    function (res) {
                                        if (res.status === 401 || res.status === 403) logout();
                                    });
                            break;
                        case "owner":
                        case "dealer_admin":
                            dEditor.option("disabled", false);
                            form.itemOption("ownerId", "label", { text: "Дилер"});
                            $.getJSON("/api/users/getAllDealers", null)
                                .then(function (data) {
                                        ownerEditor.option('dataSource', data);
                                        //if (data && data.length > 0) {
                                        //    dEditor.option('value', data[0]);
                                        //}
                                    },
                                    function (res) {
                                        if (res.status === 401 || res.status === 403) logout();
                                    });
                            break;
                        case "dealer":
                            dEditor.option("disabled", true);
                            break;
                        case "admin":
                            dEditor.option("disabled", true);
                            break;
                        }
                        //$.get("/api/users/getDealers/" + data.value)
                        //    .then(function(data) {
                        //        $("#dealer").dxSelectBox('instance').option('dataSource', data);
                        //    });
                    }
                }
            },
            {
                dataField: "ownerId",
                editorType: "dxSelectBox",
                label: {
                    text: "Дилер"
                },
                editorOptions: {
                    //dataSource: timeZones,
                    displayExpr: "name",
                    valueExpr: "id",
                    onInitialized: function(e) {
                        ownerEditor = e.component;
                    }
                }
            }
        ]
    }).dxForm("instance");
}
