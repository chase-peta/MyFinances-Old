Number.prototype.format = function (n, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
};

$(function () {
    var intVal = function(num) {
        return typeof num === 'string' ?
        num.replace(/[\$,]/g,'') * 1 :
        typeof num === 'number' ?
            num : 0;
    }

    var year;

    if ($("#loanPaymentId")) {
        var months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
            lpId = $("#loanPaymentId").val(),
            rStr = "#LoanPayment-" + lpId,
            currentPrincipal = intVal($(rStr).attr("data-current-principal")),

            $fDatePaid = $("#DatePaid"),
            $fInterest = $("#Interest"),
            $fEscrow = $("#Escrow"),
            $fBase = $("#BasicPayment"),
            $fAdd = $("#AddPayment"),

            $rDatePaid = $(rStr + " .datePaid"),
            $rInterest = $(rStr + " .interest"),
            $rEscrow = $(rStr + " .escrow"),
            $rBase = $(rStr + " .base"),
            $rAdd = $(rStr + " .add"),
            $rPayment = $(rStr + " .payment"),
            $rPrincipal = $(rStr + " .principal");

        year = $(rStr + " th").attr("data-search"),
        $(rStr).removeClass("alert-danger").removeClass("alert-success").removeClass("alert-warning").addClass("alert-info");

        function calc() {
            var pri = intVal($fAdd.val()) + intVal($fBase.val()),
                payment = pri + intVal($fEscrow.val()) + intVal($fInterest.val()),
                principal = currentPrincipal - pri;
            $rPayment.text('$' + payment.format(2));
            $rPrincipal.text('$' + principal.format(2));
        }

        $fDatePaid.keyup(function () {
            var datePaid = new Date($(this).val() + " 00:00");
            $rDatePaid.text(months[datePaid.getMonth()] + ' - ' + datePaid.getDate());
        });

        $fInterest.keyup(function () { $rInterest.text('$' + intVal($(this).val()).format(2)); calc(); })
            .focus(function () { $(this).select(); });

        $fEscrow.keyup(function () { $rEscrow.text('$' + intVal($(this).val()).format(2)); calc(); })
            .focus(function () { $(this).select(); });

        $fBase.keyup(function () { $rBase.text('$' + intVal($(this).val()).format(2)); calc(); })
            .focus(function () { $(this).select(); });

        $fAdd.keyup(function () { $rAdd.text('$' + intVal($(this).val()).format(2)); calc(); })
            .focus(function () { $(this).select(); });
    }

    var $billPaymentHistoryTable = $("#BillPaymentHistoryTable"),
        $billMonthlyAveragesTable = $("#BillMonthlyAveragesTable"),
        $loanPaymentHistoryTable = $("#LoanPaymentHistoryTable"),
        $loanPaymentOutlookTable = $("#LoanPaymentOutlookTable");

    $(".nav-tabs li a").click(function(e) {
        e.preventDefault();
        $(this).tab;
        var thisTab = $(this).attr("aria-controls");
        if (thisTab == "billPaymentHistory") {
            loadBillPaymentHistoryTable();
        } else if (thisTab == "billMonthlyAverages") {
            loadBillMonthlyAveragesTable();
        } else if (thisTab == "loanPaymentHistory") {
            loadLoanPaymentHistoryTable();
        } else if (thisTab == "loanPaymentOutlook") {
            loadLoanPaymentOutlookTable();
        }
    });
    $(".nav-tabs li.active a").click();

    if ($billPaymentHistoryTable) {
        loadBillPaymentHistoryTable();
    }
    if ($billMonthlyAveragesTable) {
        loadBillMonthlyAveragesTable();
    }

    function loadBillPaymentHistoryTable() {
        if (!$.fn.DataTable.isDataTable("#BillPaymentHistoryTable")) {
            $billPaymentHistoryTable = $billPaymentHistoryTable.DataTable({
                info: false,
                paging: false,
                ordering: false
            });
            
            $("#billPaymentHistory .pagination>li>a").click(function(e) {
                e.preventDefault();
                $billPaymentHistoryTable.column(0).search($(this).attr("data-year")).draw();
                $("#billPaymentHistory .pagination>li").removeClass("active");
                $(this).parent("li").addClass("active");
            });
            $("#billPaymentHistory .pagination>li:first()>a").click();
        }
    }

    function loadBillMonthlyAveragesTable() {
        if (!$.fn.DataTable.isDataTable("#BillMonthlyAveragesTable")) {
            $billMonthlyAveragesTable = $billMonthlyAveragesTable.DataTable({
                info: false,
                paging: false,
                search: false
            });
        }
    }

    function loadLoanPaymentHistoryTable() {
        if (!$.fn.DataTable.isDataTable("#LoanPaymentHistoryTable")) {
            $loanPaymentHistoryTable = $loanPaymentHistoryTable.DataTable({
                info: false,
                paging: false,
                ordering: false
            });

            $("#loanPaymentHistory .pagination>li>a").click(function(e) {
                e.preventDefault();
                $loanPaymentHistoryTable.column(0).search($(this).attr("data-year")).draw();
                $("#loanPaymentHistory .pagination>li").removeClass("active");
                $(this).parent("li").addClass("active");
            });
            if (year) {
                $("#loanPaymentHistory .pagination>li>a[data-year=" + year + "]").click();
            } else {
                $("#loanPaymentHistory .pagination>li:first()>a").click();
            }
        }
    }

    function loadLoanPaymentOutlookTable() {
        if (!$.fn.DataTable.isDataTable("#LoanPaymentOutlookTable")) {
            $loanPaymentOutlookTable = $loanPaymentOutlookTable.DataTable({
                info: false,
                paging: false,
                ordering: false
            });

            $("#loanPaymentOutlook .pagination>li>a").click(function(e) {
                e.preventDefault();
                $loanPaymentOutlookTable.column(0).search($(this).attr("data-year")).draw();
                $("#loanPaymentOutlook .pagination>li").removeClass("active");
                $(this).parent("li").addClass("active");
            });
            $("#loanPaymentOutlook .pagination>li:first()>a").click();
        }
    }

    $('.nav-tabs > li > a').on('click', function (e) {
        var target = $(this).attr('href');
        if (target.indexOf('gragh') > -1) {
            var chartNumber = $(this).attr('data-chart-number') - 1;
            if (charts[chartNumber].loaded == false) {
                setTimeout(function () {
                    var chart = new CanvasJS.Chart(charts[chartNumber].chartName, {
                        theme: "theme3",
                        animationEnabled: true,
                        animationDuration: 500,
                        data: [
                            {
                                type: "pie",
                                showInLegend: false,
                                toolTipContent: "{y} - #percent %",
                                yValueFormatString: "$#0.00",
                                legendText: "{indexLabel}",
                                dataPoints: charts[chartNumber].data
                            }
                        ]
                    });
                    chart.render();
                    charts[chartNumber].loaded = true;
                }, 250);
            }
        }
    });

    if (typeof (charts) !== 'undefined') {
        var chartCount = charts.length,
            i = 0;
        for (i = 0; i < chartCount; i++) {
            if (charts[i].loadOnStart == true) {
                var chart = new CanvasJS.Chart(charts[i].chartName, {
                    theme: "theme3",
                    animationEnabled: true,
                    animationDuration: 500,
                    data: [
                        {
                            type: "pie",
                            showInLegend: false,
                            toolTipContent: "{y} - #percent %",
                            yValueFormatString: "$#0.00",
                            legendText: "{indexLabel}",
                            dataPoints: charts[i].data
                        }
                    ]
                });
                chart.render();
                charts[i].loaded = true;
            }
        }
    }

});