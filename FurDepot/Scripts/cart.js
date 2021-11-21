$(document).ready(() => {
	populateCartDisplay();
});

const placeholderStorageCart = {
	'123': {
		'imgUrl': 'https://tse3.mm.bing.net/th?id=OIP.gjrPwlqp_HZCvZMDB2s47QHaHV&pid=Api',
		'name': 'Cool Used Cat Toy :3',
		'cost': 1.50,
		'quantity': 1,
	},
};

const populateCartDisplay = () => {
	const cartItems = JSON.parse(window.localStorage.getItem('cart'));
	const prototype = $('#cartItemPrototype');
	const cartItemContainer = $("#cartItemContainer");
	if (!cartItemContainer) {
		return; // not on cart page
	}
	if (!cartItems || Object.keys(cartItems).length === 0) {
		cartItemContainer.text('There are no items in your cart.');
		return;
	}

	let cartTotal = 0;

	for (let productId in cartItems) {
		const cartItem = cartItems[productId];
		const cartItemElement = $(prototype.clone());

		cartItemElement.attr('style', false);
		cartItemElement.find('.cartItemImage').attr('src', cartItem.imgUrl);
		cartItemElement.find('.cartItemName').text(cartItem.name);
		cartItemElement.find('.cartItemCost').text(parseFloat(cartItem.cost).toFixed(2));
		cartItemElement.find('.cartItemQuantity').text(cartItem.quantity);
		cartItemContainer.append(cartItemElement);

		cartTotal += cartItem.cost * cartItem.quantity;
	};

	$("#cartItemTotals .cartItemTotal").text('$' + parseFloat(cartTotal).toFixed(2));
};

$('#addToCart').on('click tap', function () {
	let currentCart = JSON.parse(window.localStorage.getItem('cart'));
	if (currentCart === null) {
		currentCart = {};
	}
	const addToCartButton = $(this);
	const productObject = {
		imgUrl: 'data:Image\/' + addToCartButton.data('image-data-file-extension') + ';base64,' + addToCartButton.data('image-bytes-base-64'),
		name: addToCartButton.data('product-name'),
		cost: addToCartButton.data('cost'),
		quantity: 1 // todo figure out what we want to do with quantity
	}
	console.log(productObject);
	currentCart[addToCartButton.data('product-id')] = productObject;
	localStorage.setItem('cart', JSON.stringify(currentCart));
})

const placeholderStorageCartMany = {
	"123": {
		"imgUrl": "https://tse3.mm.bing.net/th?id=OIP.gjrPwlqp_HZCvZMDB2s47QHaHV&pid=Api",
		"name": "Cool Used Cat Toy :3",
		"cost": 1.50,
		"quantity": 1
	},
	"456": {
		"imgUrl": "https://yourpetsneed.com/wp-content/uploads/2020/04/best-cat-toys-featured.jpg",
		"name": "Cooler Used Cat Toy >:3",
		"cost": 2.20,
		"quantity": 20
	}
};