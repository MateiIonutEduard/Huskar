$(document).ready(() => {
    $('#sender').on('submit', e => {
        e.preventDefault();
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