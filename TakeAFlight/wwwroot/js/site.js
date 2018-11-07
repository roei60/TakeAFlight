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
            if(data!="Error")
                cart.AddItemToCart(data);
            else
                alert("An Error has occured adding that flight to cart, pls try again later...")
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

    $("#order_btn").on("click", function (e) {
        cart.ClearCart(e);
    });

    $("#log_out_btn").on("click", function (e) {
        cart.ClearCart(e);
    });
    
    LoadCartData();

}));
