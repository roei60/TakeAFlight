var Cart = {}
Cart.items = {};
Cart.Count = 0;

class CartManager {

    //receiver json and adding it to cart. also save the cart to localstorage
    AddItemToCart(item) {
        if (Cart.items[item.flightID] != undefined) {
            alert("this flight is already on your cart list, handle quantity on purchase!");
            return;
        }
        var tempItem = $(".cloneable-element").clone();
        tempItem.find("#FlightDest").text(item.destination.country + "," + item.destination.city);
        tempItem.find("#FlightDate").text(new Date(item.departure).toLocaleString());
        tempItem.find("#FlightPrice").text(item.price + "$");
        tempItem.find("#FlightId").val(item.flightID);
        tempItem.removeClass("cloneable-element");
        $("#CartList").append(tempItem);

        Cart.items[item.flightID] = item;
        Cart.Count++;
        localStorage.setItem("Cart", JSON.stringify(Cart));
        this.UpdateCartSize();
    }

    DeleteItemFromCart(btn) {
        var containerli = btn.target.closest("li");
        var DeletedFlightFromCart = containerli.children.FlightId.value;
        delete (Cart.items[DeletedFlightFromCart]);
        btn.target.closest("li").remove();
        Cart.Count--;

        localStorage.setItem("Cart", JSON.stringify(Cart));
        this.UpdateCartSize();

    }

    ClearCart(e) {
        for (var item in Cart.items) {
            delete (Cart.items[item]);
        }
        Cart.Count = 0;
        localStorage.setItem("Cart", JSON.stringify(Cart));
        this.UpdateCartSize();
    }

    SendDataToViewCart(e) {
        var flightsIdArray = []
     
        Object.keys(Cart.items).forEach(function (key) {
            flightsIdArray.push(parseInt(key));
        });

        $.post('/ViewCart/Index', $.param({ data: flightsIdArray }, true), function (data) { });
    }

    UpdateCartSize() {
        
      $("#CartLength").html(" "+Cart.Count+" - Items")
    }

    initCart(thatCart) {

        for (var key in thatCart.items) {
            this.AddItemToCart(thatCart.items[key]);
        }

    }
}