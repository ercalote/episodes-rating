$(document).ready(function () {
    $(document).on("click", ".ui-autocomplete a", function (e) {
        $("#loading").modal({
            escapeClose: false,
            clickClose: false,
            showClose: false,
            fadeDuration: 100
        });
    });

    $('#search_data').autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: "/Home/Search",
                cache: false,
                data: { title: request.term },
                datatype: "json",
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.label, value: item.value, id: item.id, friendly: item.friendly, year: item.year };
                    }));
                }
            });
        },
        minLength: 3,
        select: function (event, ui) {
            $('#search_data').val(ui.item.value);
        }
    }).data('ui-autocomplete')._renderItem = function (ul, item) {
        return $("<li class='ui-autocomplete-row'></li>")
            .data("item.autocomplete", item)
            .append("<a rel='modal: open' href='/" + item.id + "/" + item.friendly + "' title='" + item.value + " episodes rating'><img src='" + item.label + "' /><p>" + item.value + "<br />(" + item.year + ")</p></a><div class='clearfix'></div>")
            .appendTo(ul);
    };

    $('.rating-hoverable-row').on('mouseover', function () {
        $(this)
            .closest('tr')
            .addClass('highlight');
    });

    $('.rating-hoverable-row').on('mouseout', function () {
        $(this)
            .closest('tr')
            .removeClass('highlight');
    });

    $('.rating-hoverable-column').on('mouseover', function () {
        $(this)
            .closest('table')
            .find('.rating:nth-child(' + ($(this).index() + 1) + ')')
            .addClass('highlight');
    });

    $('.rating-hoverable-column').on('mouseout', function () {
        $(this)
            .closest('table')
            .find('.rating:nth-child(' + ($(this).index() + 1) + ')')
            .removeClass('highlight');
    });

    $("#flexisel").flexisel({
        visibleItems: 6,
        animationSpeed: 1000,
        autoPlay: true,
        autoPlaySpeed: 5000,
        pauseOnHover: false,
        enableResponsiveBreakpoints: true,
        responsiveBreakpoints: {
            portrait: {
                changePoint: 480,
                visibleItems: 1
            },
            landscape: {
                changePoint: 640,
                visibleItems: 2
            },
            tablet: {
                changePoint: 768,
                visibleItems: 3
            }
        }
    });
});