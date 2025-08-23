// Financial Dashboard JavaScript
document.addEventListener("DOMContentLoaded", () => {
    // Sidebar toggle functionality
    const sidebarToggle = document.getElementById("sidebarToggle")
    const sidebar = document.getElementById("sidebar")
    const mainContent = document.querySelector(".financial-main-content")

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener("click", () => {
            sidebar.classList.toggle("collapsed")

            // Update toggle icon
            const icon = sidebarToggle.querySelector("i")
            if (sidebar.classList.contains("collapsed")) {
                icon.className = "fas fa-chevron-right"
            } else {
                icon.className = "fas fa-chevron-left"
            }
        })
    }

    // Mobile sidebar toggle
    const navbarToggler = document.querySelector(".navbar-toggler")
    if (navbarToggler && window.innerWidth <= 768) {
        navbarToggler.addEventListener("click", () => {
            sidebar.classList.toggle("show")
        })
    }

    // Add active class to current navigation item
    const currentPath = window.location.pathname
    const navLinks = document.querySelectorAll(".sidebar-nav .nav-link")

    navLinks.forEach((link) => {
        link.classList.remove("active")
        if (
            link.getAttribute("href") === currentPath ||
            (currentPath.includes(link.getAttribute("href")) && link.getAttribute("href") !== "/")
        ) {
            link.classList.add("active")
        }
    })

    // Add fade-in animation to cards
    const cards = document.querySelectorAll(".futuristic-card")
    cards.forEach((card, index) => {
        setTimeout(() => {
            card.classList.add("fade-in")
        }, index * 100)
    })

    // Enhanced search functionality
    const searchInput = document.querySelector(".financial-search")
    if (searchInput) {
        searchInput.addEventListener("focus", function () {
            this.parentElement.style.transform = "scale(1.02)"
        })

        searchInput.addEventListener("blur", function () {
            this.parentElement.style.transform = "scale(1)"
        })
    }

    // Notification badge animation
    const notificationBadge = document.querySelector(".notification-badge")
    if (notificationBadge) {
        setInterval(() => {
            notificationBadge.style.animation = "none"
            setTimeout(() => {
                notificationBadge.style.animation = "pulse 2s infinite"
            }, 10)
        }, 5000)
    }
})

// Utility function for smooth scrolling
function smoothScrollTo(element) {
    element.scrollIntoView({
        behavior: "smooth",
        block: "start",
    })
}

// Add loading states for forms
function addLoadingState(button) {
    const originalText = button.innerHTML
    button.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Processing...'
    button.disabled = true

    return function removeLoadingState() {
        button.innerHTML = originalText
        button.disabled = false
    }
}
