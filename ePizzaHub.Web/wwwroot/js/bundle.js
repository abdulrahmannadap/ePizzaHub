function AddToCart(ItemId, Name, UnitPrice, Quantity) {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: '/Cart/AddToCart/' + ItemId + "/" + UnitPrice + "/" + Quantity,
        success: function (d) {
            var data = d.length > 0 ? JSON.parse(d) : null;
            if (data != null && data.CartItems.length > 0) {
                $("#cartCounter").text(data.CartItems.length);
                var message = '<strong>' + Name + '</strong> Added to <a href="/cart">Cart</a> Successfully!';
                $("#toastCart > .toast-body").html(message);
                $("#toastCart").toast("show");
                setTimeout(function () {
                    $("#toastCart").toast("hide");
                }, 4000);
            }
        }
    });
}

function updateQuantity(id, total, quantity) {
    if (id > 0 && quantity > 0) {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: '/Cart/UpdateQuantity/' + id + "/" + quantity,
            success: function (data) {
                if (data > 0) {
                    location.reload();
                }
            },
            error: function (result) {
            },
        });
    }
    else if (id > 0 && quantity < 0 && total > 1) {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: '/Cart/UpdateQuantity/' + id + "/" + quantity,
            success: function (data) {
                if (data > 0) {
                    location.reload();
                }
            },
            error: function (result) {
            },
        });
    }
}
function deleteItem(id) {
    if (id > 0) {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: '/Cart/DeleteItem/' + id,
            success: function (data) {
                if (data > 0) {
                    location.reload();
                }
            },
            error: function (result) {
            },
        });
    }
}

$(document).ready(function () {
    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: '/Cart/GetCartCount',
        dataType: "json",
        success: function (data) {
            $("#cartCounter").text(data);
        },
        error: function (result) {
        },
    });
});
function Add(x, y) {
    return x + y;
}