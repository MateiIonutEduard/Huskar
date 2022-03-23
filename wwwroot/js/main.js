$(document).ready(() => {
    $('#sender').on('submit', e => {
        e.preventDefault();
    });

    $('#box').on('submit', e => {
        e.preventDefault();
        var array = $('#genres').val();
        var name = $('#search').val();
        var buffer = new Array();

        if (name) buffer.push("name=" + name);

        for (var k = 0; k < array.length; k++)
            buffer.push(`filter[${k}]=${array[k]}`);

        if (buffer.length) {
            setTimeout(() => {
                var url = buffer.join("&");
                location.href = '/Home/Results/?' + url;
            }, 500);
        }
    });

    $("#filter").on("click", () => {
        var array = $('#genres').val();
        var query = new Array();

        for (var k = 0; k < array.length; k++)
            query.push(`filter[${k}]=${array[k]}`);

        if (query.length) {
            setTimeout(() => {
                var url = query.join("&");
                location.href = '/Home/Results/?' + url;
            }, 500);
        }
    });
});

function postMessage(userId) {
    var msg = $("#texting").val();
    var query = location.href;

    var url = new URL(query);
    var id = url.searchParams.get("id");

    var buffer = {
        "UserId": userId,
        "Message": msg,
        "MovieId": id
    };

    $.ajax({
        url: '/Post/Post/',
        type: 'post',
        cache: false,
        data: buffer,
        success: () => {
            setTimeout(() => {
                location.reload();
            }, 100);
        },
        async: true
    });
}

function removePost(id) {
    var buffer = {
        "PostId": id
    };

    $.ajax({
        url: '/Post/Remove/',
        type: 'delete',
        cache: false,
        data: buffer,
        success: () => {
            setTimeout(() => {
                location.reload();
            }, 100);
        },
        async: true
    });
}