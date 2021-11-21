
//Added this snippet of code; Hilbert - 11/14/2021 - still need to be modify;
//All Ajax here
function deleteImageAjax(deleteType, UserID, intID) {
	try {
		var ajaxData = { //json structure
			//Two Properties
			UserID: UserID,
			intID: intID
		};

		var strURL;
		if (deleteType == "profile")
			//Http post of  DeleteImage()
			strURL = "../Profile/DeleteImage";
		else //event
			strURL = "../../Profile/DeleteProductImage";
		//Ajax function is done asynchronously 
		$.ajax({
			type: "POST",
			url: strURL,
			data: ajaxData,
			success: function (returnData) {
				//If it returns a one then it means that it deleted that one record 
				if (returnData.Status == 1) {
					//hide the image
					$("#image-".concat(intID)).hide(); // ex:image-213
				}
				else {
					alert('Unable to remove image.');
				}
			},
			error: function (xhr) {
				debugger;
			}
		});
	}
	catch (err) {
		showError(err);
	}
}






//Drag and Drop 
//All JQuery here


