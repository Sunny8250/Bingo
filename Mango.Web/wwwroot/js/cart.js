
// Call the function initially when the page loads
$(document).ready(function () {
    updateCartCount();
});
function updateCartCount() {
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