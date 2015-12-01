$(function() {
    var intVal = function(num) {
        return typeof num === 'string' ?
        num.replace(/[\$,]/g,'') * 1 :
        typeof num === 'number' ?
            num : 0;
    }

    var $billPaymentHistoryTable = $("#BillPaymentHistoryTable"),
        $billMonthlyAveragesTable = $("#BillMonthlyAveragesTable"),
        $loanPaymentHistoryTable = $("#LoanPaymentHistoryTable"),
        $loanPaymentOutlookTable = $("#LoanPaymentOutlookTable"),
        selectedYear = false;

    $(".nav-tabs li a").click(function(e) {
        e.preventDefault();
        $(this).tab;
        selectedYear = false;
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
        var selectedYear = false;

        if (!$.fn.DataTable.isDataTable("#BillPaymentHistoryTable")) {
            $billPaymentHistoryTable = $billPaymentHistoryTable.DataTable({
                info: false,
                paging: false,
                order: [[0,"asc"]],
                "bSortClasses": false,
                footerCallback: function(row,data,start,end,display) {

                    if (selectedYear) {
                        var total = 0.00,
                            i = 0,
                            displayLength = display.length;

                        for (var i = 0;i < displayLength;i++) {
                            total += intVal(data[display[i]][3]);
                        }
                        total = ((total * 100) + "").split(".")[0] / 100;
                        $(row).find(".total").html("$" + total);
                    }
                }
            });
            
            $("#billPaymentHistory .pagination>li>a").click(function(e) {
                e.preventDefault();
                selectedYear = true;
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
                order: [[0,"desc"]],
                "bSortClasses": false,
                footerCallback: LoanOnComplete
            });

            $("#loanPaymentHistory .pagination>li>a").click(function(e) {
                e.preventDefault();
                selectedYear = true;
                $loanPaymentHistoryTable.column(0).search($(this).attr("data-year")).draw();
                $("#loanPaymentHistory .pagination>li").removeClass("active");
                $(this).parent("li").addClass("active");
            });
            $("#loanPaymentHistory .pagination>li:first()>a").click();
        }
    }

    function loadLoanPaymentOutlookTable() {
        if (!$.fn.DataTable.isDataTable("#LoanPaymentOutlookTable")) {
            $loanPaymentOutlookTable = $loanPaymentOutlookTable.DataTable({
                info: false,
                paging: false,
                order: [[0,"asc"]],
                "bSortClasses": false,
                footerCallback: LoanOnComplete
            });

            $("#loanPaymentOutlook .pagination>li>a").click(function(e) {
                e.preventDefault();
                selectedYear = true;
                $loanPaymentOutlookTable.column(0).search($(this).attr("data-year")).draw();
                $("#loanPaymentOutlook .pagination>li").removeClass("active");
                $(this).parent("li").addClass("active");
            });
            $("#loanPaymentOutlook .pagination>li:first()>a").click();
        }
    }

    function LoanOnComplete(row,data,start,end,display) {
        if (selectedYear) {
            var interestTotal = 0.00,
                escrowTotal = 0.00,
                baseTotal = 0.00,
                addTotal = 0.00,
                paymentTotal = 0.00,
                i = 0,
                displayLength = display.length;

            for (var i = 0;i < displayLength;i++) {
                interestTotal += intVal(data[display[i]][1]);
                escrowTotal += intVal(data[display[i]][2]);
                baseTotal += intVal(data[display[i]][3]);
                addTotal += intVal(data[display[i]][4]);
                paymentTotal += intVal(data[display[i]][5]);
            }
            interestTotal = ((interestTotal * 100) + "").split(".")[0] / 100;
            escrowTotal = ((escrowTotal * 100) + "").split(".")[0] / 100;
            baseTotal = ((baseTotal * 100) + "").split(".")[0] / 100;
            addTotal = ((addTotal * 100) + "").split(".")[0] / 100;
            paymentTotal = ((paymentTotal * 100) + "").split(".")[0] / 100;
            var total = (((addTotal + baseTotal) * 100) + "").split(".")[0] / 100;

            $(row).find(".interestTotal").html("$" + interestTotal);
            $(row).find(".escrowTotal").html("$" + escrowTotal);
            $(row).find(".baseTotal").html("$" + baseTotal);
            $(row).find(".addTotal").html("$" + addTotal);
            $(row).find(".paymentTotal").html("$" + paymentTotal);
            $(row).find(".total").html("$" + total);
        }
    }

    if ($("#LoanPaymentHelper")) {

        var $currentPrincipal = $("#currentPrincipal"),
            $basePayment = $("#basePaymentHelper"),
            $addPayment = $("#addPaymentHelper"),
            $totalPayment = $("#totalPayment"),
            $newPrincipal = $("#newPrincipal"),
            totalPayment = 0.00;

        function calculateNewBalance() {
            totalPayment = intVal($basePayment.text()) + intVal($addPayment.text())
            $totalPayment.text("$" + totalPayment);
            $newPrincipal.text("$" + (intVal($currentPrincipal.text()) - totalPayment));
        }
        calculateNewBalance();

        var $textBoxBasePayment = $("#BasicPayment"),
            $textBoxAddPayment = $("#AddPayment");

        $textBoxBasePayment.keyup(function() {
            $basePayment.text($(this).val());
            calculateNewBalance();
        });
        $textBoxAddPayment.keyup(function() {
            $addPayment.text($(this).val());
            calculateNewBalance();
        });
    }

});