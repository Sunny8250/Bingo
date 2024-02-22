
// Call the function initially when the page loads
$(document).ready(function () {
    getCartCount();
    /*removeCartItem();*/
});
function getCartCount() {
    $.ajax({
        url: '/Cart/GetCartCount', // This should be the URL of your endpoint to fetch cart count
        type: 'GET',
        success: function (response) {
            $('#cart-count').text(response); // Update the cart count in the span
            //localStorage.setItem('cartCount', response);
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            // Handle errors if any
        }
    });
}

//function removeCartItem() {
//    $('.remove-cart-item').click(function () {
//        var cartDetailsId = $(this).attr('cartDetailsId');
//        var cartDTO = $(this).attr('model');
//        console.log(this);
//        var $clickedButton = $(this);

//        //Make the Ajax call
//        $.ajax({
//            url: '/Cart/Remove',
//            type: 'POST',
//            data: {
//                cartDetailsId: cartDetailsId,
//                cartDTO: cartDTO
//            },
//            success: function (response) {
//                if (response.isSuccess) {
//                    // Update the UI (remove the cart item from the view)
//                    $clickedButton.closest('.cart-item').remove();
//                    // Show success message (if needed)
//                    alert("Cart item removed successfully.");
//                } else {
//                    // Show error message (if needed)
//                    alert("Failed to remove cart item.");
//                }
//            },
//            error: function (xhr, status, error) {
//                // Handle error response
//                console.error(xhr.responseText);
//                // Show error message (if needed)
//                alert("Error occurred while removing cart item.");
//            }
//        });
//    });
//}

// // Function to retrieve and update the cart count
// function retrieveAndUpdateCartCount()
// {
//     var storedCartCount = localStorage.getItem('cartCount');
//     var logOutBtn = document.getElementById('logout');
//     if (storedCartCount != 0 && logOutBtn != null) {
//     $('#cart-count').text(storedCartCount); // Update the cart count in the span
//     }
//     else
//     {
//         localStorage.clear();
//         updateCartCount(); // Fetch and update the cart count
//     }
// }