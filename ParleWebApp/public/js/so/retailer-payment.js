$(document).ready(function() {

    $("#filtercontent").click(function() {
        $("#filtermonth").toggle();

    });
    $(".applybtn-filter").click(function() {
        $("#filtermonth").hide();
    });
    $(".closebtn-filter").click(function() {
        $("#filtermonth").hide();
    });
});