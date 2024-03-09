var createProjectBtn = document.getElementById('create-project-btn')
var createProjectModal = document.getElementById('create-project-modal')

createProjectBtn.addEventListener('click', function () {
    createProjectModal.classList.add('active')
})

function closeModal() {
    createProjectModal.classList.remove('active')
}
