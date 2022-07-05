(function (hangfire) {
    hangfire.Management = (function () {
        function Management() {
            this._initialize();
        }
        Management.prototype._initialize = function () {
            $('.js-management').each(function () {
                var container = this;

                var showCommandsPanelOptions = function (commandsType) {
                    $(".commands-panel", container).hide();
                    $(".commands-panel." + commandsType, container).show();

                    $(".commands-options", container).hide();
                    $(".commands-options." + commandsType, container).show();
                    //data-commands-type="Enqueue" data-id="@(id)"
                    $(".commandsType." + id).html($("a[data-commands-type='" + commandsType + "']", container).html());
                };

                var $this = $(this);
                var id = $this.data("id");
                showCommandsPanelOptions("Enqueue");

                $(this).on('click', '.commands-type',
                    function (e) {
                        var $this = $(this);
                        var commandsType = $this.data('commands-type');
                        showCommandsPanelOptions(commandsType);
                        e.preventDefault();
                    });

                $(this).on('click', '.js-management-input-CronModal',
                    function (e) {
                        var $this = $(this);
                        var id = $this.attr("input-id");
                        var cron = $("#" + id + "_sys_cron").val();
                        $("#result").val(cron || "* * * * *").data("cronId", id);
                        $('#analysis').click()
                        $('#cronModal').modal("show")
                        e.preventDefault();
                    });
                $("#connExpressionOk").click(function () {
                    var id = $("#result").data("cronId");
                    var cron = $("#result").val();
                    $("#" + id + "_sys_cron").val(cron);
                    $('#cronModal').modal("hide")
                })
                $(this).on('click', '.js-management-input-commands',
                    function (e) {
                        var $this = $(this);
                        var confirmText = $this.data('confirm');

                        var id = $this.attr("input-id");
                        var type = $this.attr("input-type");
                        var send = { id: id, type: type };

                        $("input[id^='" + id + "']", container).each(function () {
                            send[$(this).attr('id')] = $(this).val();
                        });
                        $("textarea[id^='" + id + "']", container).each(function () {
                            send[$(this).attr('id')] = $(this).val();
                        });
                        $("select[id^='" + id + "']", container).each(function () {
                            send[$(this).attr('id')] = $(this).val();
                        });
                        $("div[id^='" + id + "']", container).each(function () {
                            send[$(this).attr('id')] = $(this).data('date');
                        });

                        if (!!$this.attr('schedule')) {
                            send['schedule'] = $this.attr("schedule");
                        }

                        if (!confirmText || confirm(confirmText)) {
                            $this.prop('disabled');
                            var loadingDelay = setTimeout(function () {
                                $this.button('loading');
                            }, 100);

                            $.post($this.data('url'), send, function () {
                                clearTimeout(loadingDelay);
                                $this.removeProp('disabled');
                                $this.button('reset');
                                window.location.reload();
                            }).fail(function (xhr, status, error) {
                                var errorMsg = xhr.getResponseHeader("errorMsg");
                                Hangfire.Management.alert(id, errorMsg || "There was an error. " + error);
                                $this.removeProp('disabled');
                                $this.button('reset');
                            });
                        }

                        e.preventDefault();
                    });
            });
        };

        Management.alert = function (id, message) {
            $('.' + id + '_error')
                .html('<div class="alert alert-danger"><a class="close" data-dismiss="alert">×</a><strong>Error! </strong><span>' +
                    message +
                    '</span></div>');
        }

        return Management;
    })();
})(window.Hangfire = window.Hangfire || {});

function loadManagement() {
    Hangfire.management = new Hangfire.Management();

    var link = document.createElement('link');
    link.setAttribute("rel", "stylesheet");
    link.setAttribute("type", "text/css");
    link.setAttribute("href", 'https://cdn.bootcdn.net/ajax/libs/bootstrap-datetimepicker/4.17.47/css/bootstrap-datetimepicker.min.css');
    document.getElementsByTagName("head")[0].appendChild(link);
    var url = "https://cdn.bootcdn.net/ajax/libs/bootstrap-datetimepicker/4.17.47/js/bootstrap-datetimepicker.min.js";
    $.getScript(url,
        function () {
            $(function () {
                //$("div[id$='_datetimepicker']").datetimepicker({ format: "YYYY-MM-DD HH:mm:ss" });
                $("input.date").datetimepicker({ format: "YYYY-MM-DD HH:mm:ss" });
                $("div.date").datetimepicker({ format: "YYYY-MM-DD HH:mm:ss" });
                //$('input.time').datetimepicker({ pickDate: false });
            });
        });

    //var link2 = document.createElement('link');
    //link2.setAttribute("rel", "stylesheet");
    //link2.setAttribute("type", "text/css");
    //link2.setAttribute("href", 'https://cdn.bootcss.com/bootstrap-timepicker/0.5.2/css/bootstrap-timepicker.min.css');
    //document.getElementsByTagName("head")[0].appendChild(link2);
    //var url2 = "https://cdn.bootcss.com/bootstrap-timepicker/0.5.2/js/bootstrap-timepicker.min.js";
    //$.getScript(url2,
    //    function () {
    //        $(function () {
    //            $('input.time').timepicker();    //        });
    //    });

    //var link2 = document.createElement('link');
    //link2.setAttribute("rel", "stylesheet");
    //link2.setAttribute("type", "text/css");
    //link2.setAttribute("href", 'https://cdn.bootcss.com/jquery.inputmask/3.3.4/css/inputmask.min.css');
    //document.getElementsByTagName("head")[0].appendChild(link2);
    var url2 = "https://cdn.bootcdn.net/ajax/libs/jquery.inputmask/5.0.6/jquery.inputmask.js";
    $.getScript(url2,
        function () {
            $(function () {
                //$("div[id$='_datetimepicker']").datepicker();
                //$('input.date').datepicker();
                //$('input.time').inputmask();                $('input[data-inputmask]').inputmask();                $('textarea[data-inputmask]').inputmask();            });
        });

    //var link3 = document.createElement('link');
    //link3.setAttribute("rel", "stylesheet");
    //link3.setAttribute("type", "text/css");
    //link3.setAttribute("href", 'https://cdn.bootcss.com/fuelux/3.16.3/css/fuelux.css');
    //document.getElementsByTagName("head")[0].appendChild(link3);
    //var url3 = "https://cdn.bootcss.com/fuelux/3.16.3/js/fuelux.js";
    //$.getScript(url3,
    //    function () {
    //        $(function () {
    //            $('input[type=number]').addClass("spinbox-input").spinbox();    //        });
    //    });
    var link3 = document.createElement('link');
    link3.setAttribute("rel", "stylesheet");
    link3.setAttribute("type", "text/css");
    link3.setAttribute("href", 'https://cdn.bootcdn.net/ajax/libs/chosen/1.8.7/chosen.css');
    document.getElementsByTagName("head")[0].appendChild(link3);
    var url3 = "https://cdn.bootcdn.net/ajax/libs/chosen/1.8.7/chosen.jquery.js";
    $.getScript(url3,
        function () {
            $(function () {
                $('select').chosen({
                    no_results_text: "未找到此选项",
                    width: "100%"
                });            });
        });
}

if (window.attachEvent) {
    window.attachEvent('onload', loadManagement);
} else {
    if (window.onload) {
        var curronload = window.onload;
        var newonload = function (evt) {
            curronload(evt);
            loadManagement(evt);
        };
        window.onload = newonload;
    } else {
        window.onload = loadManagement;
    }
}