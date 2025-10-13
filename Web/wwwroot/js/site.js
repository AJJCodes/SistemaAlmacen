//  Lógica dropdowns (solo uno abierto a la vez)
document.querySelectorAll('.sidebar .dropdown-toggle').forEach(toggle => {
    toggle.addEventListener('click', function (e) {
        e.preventDefault();
        const parent = this.closest('.dropdown');
        document.querySelectorAll('.sidebar .dropdown').forEach(d => {
            if (d !== parent) d.classList.remove('show');
        });
        parent.classList.toggle('show');
    });
});

// Cerrar dropdowns al hacer clic fuera del sidebar
document.addEventListener('click', function (e) {
    if (!e.target.closest('.sidebar')) {
        document.querySelectorAll('.sidebar .dropdown').forEach(d => d.classList.remove('show'));
    }
});

//  Mostrar / ocultar sidebar
const toggleBtn = document.getElementById('toggleSidebar');
const sidebar = document.getElementById('sidebar');

toggleBtn.addEventListener('click', () => {
    sidebar.classList.toggle('collapsed');
});

//Inicializar todos los selectpicker
$('select').selectpicker();
