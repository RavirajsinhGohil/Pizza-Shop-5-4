// For toggle the side tabs of menu categories and modifier groups
function toggleTabs() {
    var sideTabs = document.querySelectorAll(".side-tab");
    sideTabs.forEach(function (tab) {
        if (tab.style.display === "none" || tab.style.display === "") {
            tab.style.display = "block";
        } else {
            tab.style.display = "none";
        }
    });
}

// Get the data for Edit Category
var editButtons = document.querySelectorAll('.btn-edit');
editButtons.forEach(function (button) {
    button.addEventListener('click', function () {
        var categoryId = button.getAttribute('data-id');
        var categoryName = button.getAttribute('data-name');
        var categoryDescription = button.getAttribute('data-description');
        var modal = document.getElementById('editCategory_' + categoryId);
        modal.querySelector('[name="Categoryid"]').value = categoryId;
        modal.querySelector('[name="Name"]').value = categoryName;
        modal.querySelector('[name="Description"]').value = categoryDescription;
    });
});



// Get the data for Edit Modifier Group
// var editButtonsModifiers = document.querySelectorAll('.btn-edit-modifier');
// editButtonsModifiers.forEach(function (button) {
//     button.addEventListener('click', function () {
//         var ModifierGroupId = button.getAttribute('data-id');
//         var modifierGroupName = button.getAttribute('data-name');
//         var modifierGroupDescription = button.getAttribute('data-description');
//         var existingModifiers = JSON.parse(button.getAttribute('data-existingModifiers'));
//         JSON.parse(atob(button.getAttribute()))
//         var modal = document.getElementById('editModifierGroup_' + ModifierGroupId);
//         modal.querySelector('[name="ModifierGroupId"]').value = ModifierGroupId;
//         modal.querySelector('[name="modifierGroupName"]').value = modifierGroupName;
//         modal.querySelector('[name="modifierGroupDescription"]').value = modifierGroupDescription;
//         modal.querySelector('[name="existingModifiers"]').value = existingModifiers
//     });
// });;


