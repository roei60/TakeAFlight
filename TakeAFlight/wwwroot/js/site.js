// Write your JavaScript code.
let cart = new CartManager();
console.log(localStorage.getItem("Cart"))

if ($(document).ready(function () {

    function LoadCartData() {
        var cartObj = JSON.parse(localStorage.getItem("Cart"));
        cart.initCart(cartObj); 
    }

    $("td").on("click", "#AddToCartLink", function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        var param = url.substring(url.lastIndexOf("/"));
        param = param.slice(1, param.length);

        //ajax request for getting flight data
        $.post($(this).attr("href"), { FlightId: param }, function (data, status) {
            console.log(data);
            cart.AddItemToCart(data);
        });
    })
    //avoid the list from being close on click
    $("li").on("click", ".item", function (e) {
        event.stopPropagation();
    });

    $("#CartList").on("click", "#RemoveCartItem", function (e) {
        cart.DeleteItemFromCart(e);
        event.stopPropagation();
    });

    $("#ViewCart").on("click", function (e) {
        //event.stopPropagation();
        cart.SendDataToViewCart(e);

    });

    LoadCartData();

}));
