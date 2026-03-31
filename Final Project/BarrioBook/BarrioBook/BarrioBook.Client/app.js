// ===== CONFIGURACIÓN =====
const API_BASE = "https://localhost:7178/api"; // cambia el puerto si tu API usa otro
const API_ORIGIN = API_BASE.replace(/\/api\/?$/, "");

let authToken = null;
let currentCustomer = null;

// ===== HELPERS DE UI =====
const $ = (selector) => document.querySelector(selector);
const $$ = (selector) => Array.from(document.querySelectorAll(selector));

function show(element) {
    element.classList.remove("hidden");
}
function hide(element) {
    element.classList.add("hidden");
}

function setText(el, text) {
    el.textContent = text ?? "";
}

// ===== FETCH GENÉRICO =====
async function apiRequest(path, { method = "GET", body = null, auth = true } = {}) {
    const headers = {};
    if (body !== null) {
        headers["Content-Type"] = "application/json";
    }
    if (auth && authToken) {
        headers["Authorization"] = `Bearer ${authToken}`;
    }

    const response = await fetch(`${API_BASE}${path}`, {
        method,
        headers,
        body: body ? JSON.stringify(body) : null,
    });

    if (!response.ok) {
        let message = `Error HTTP ${response.status}`;
        try {
            const errorData = await response.json();
            if (errorData?.error) message = errorData.error;
            if (errorData?.message) message = errorData.message;
        } catch {
            
        }
        throw new Error(message);
    }

    if (response.status === 204) return null;
    return response.json();
}

async function handleLogin(event) {
    event.preventDefault();
    const email = $("#loginEmail").value.trim();
    const password = $("#loginPassword").value.trim();
    const errorEl = $("#loginError");
    setText(errorEl, "");

    try {
        const result = await apiRequest("/auth/login", {
            method: "POST",
            body: { email, password },
            auth: false,
        });

        authToken = result.token;
        currentCustomer = result.customer;

        updateAuthUI();
        await loadAllData();
    } catch (err) {
        console.error(err);
        setText(errorEl, err.message || "Credenciales inválidas");
    }
}

async function handleRegister(event) {
    event.preventDefault();
    const name = $("#registerName").value.trim();
    const phone = $("#registerPhone").value.trim();
    const email = $("#registerEmail").value.trim();
    const password = $("#registerPassword").value.trim();
    const errorEl = $("#registerError");
    setText(errorEl, "");

    try {
        await apiRequest("/auth/register", {
            method: "POST",
            body: { name, phone, email, password },
            auth: false,
        });

        const loginResult = await apiRequest("/auth/login", {
            method: "POST",
            body: { email, password },
            auth: false,
        });

        authToken = loginResult.token;
        currentCustomer = loginResult.customer;

        updateAuthUI();
        await loadAllData();
    } catch (err) {
        console.error(err);
        setText(errorEl, err.message || "No se pudo registrar el usuario");
    }
}

function updateAuthUI() {
    if (authToken && currentCustomer) {
        hide($("#authView"));
        show($("#appView"));
        show($("#logoutBtn"));

        setText(
            $("#userInfo"),
            `${currentCustomer.name} (${currentCustomer.email ?? "sin email"})`
        );
    } else {
        show($("#authView"));
        hide($("#appView"));
        hide($("#logoutBtn"));
        setText($("#userInfo"), "No has iniciado sesión");
    }
}

function handleLogout() {
    authToken = null;
    currentCustomer = null;
    updateAuthUI();
}

function setupTabs() {
    const tabs = $$(".tab");
    tabs.forEach((tab) => {
        tab.addEventListener("click", () => {
            const viewId = tab.dataset.view;

            tabs.forEach((t) => t.classList.remove("active"));
            tab.classList.add("active");

            $$(".view").forEach((v) => v.classList.remove("active"));
            $(`#${viewId}`).classList.add("active");
        });
    });
}

async function loadBooks() {
    const tbody = $("#booksTable tbody");
    tbody.innerHTML = "<tr><td colspan='8'>Cargando...</td></tr>";

    try {
        const response = await apiRequest(
            "/books/List?PageNumber=1&PageSize=50",
            { auth: false } 
        );

        const page = response.data;
        const items = page?.items ?? [];

        if (!items.length) {
            tbody.innerHTML = "<tr><td colspan='8'>No hay libros registrados.</td></tr>";
            return;
        }

        tbody.innerHTML = "";
        for (const book of items) {
            const tr = document.createElement("tr");

            const hasImage = book.imageUrl && book.imageUrl.trim() !== "";
            const imgCell = hasImage
                ? `<img src="${book.imageUrl}" alt="Portada" class="book-thumb" />`
                : "<span class='badge'>Sin imagen</span>";

            tr.innerHTML = `
                <td>${book.id}</td>
                <td>${book.title}</td>
                <td>${book.author}</td>
                <td>$${book.salePrice.toFixed(2)}</td>
                <td>${book.currentStock}</td>
                <td>${book.supplierId ?? "-"}</td>
                <td>${imgCell}</td>
                <td>
                    <button class="btn btn-small btn-outline" data-edit="${book.id}">Editar</button>
                    <button class="btn btn-small btn-secondary" data-delete="${book.id}">Borrar</button>
                </td>
            `;
            tbody.appendChild(tr);
        }

        tbody.querySelectorAll("[data-edit]").forEach((btn) => {
            btn.addEventListener("click", () => {
                const id = parseInt(btn.dataset.edit, 10);
                const book = items.find((b) => b.id === id);
                if (!book) return;
                fillBookForm(book);
            });
        });

        tbody.querySelectorAll("[data-delete]").forEach((btn) => {
            btn.addEventListener("click", async () => {
                const id = parseInt(btn.dataset.delete, 10);
                if (!confirm(`¿Eliminar libro #${id}?`)) return;
                try {
                    await apiRequest(`/books/${id}/Delete`, { method: "DELETE" });
                    await loadBooks();
                } catch (err) {
                    alert(err.message || "No se pudo eliminar el libro");
                }
            });
        });

        tbody.querySelectorAll(".book-thumb").forEach((img) => {
            img.addEventListener("click", () => {
                window.open(img.src, "_blank");
            });
        });
    } catch (err) {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan='8'>Error al cargar libros: ${err.message}</td></tr>`;
    }
}

function fillBookForm(book) {
    $("#bookId").value = book.id;
    $("#bookTitle").value = book.title;
    $("#bookAuthor").value = book.author;
    $("#bookPrice").value = book.salePrice;
    $("#bookStock").value = book.currentStock;
    $("#bookSupplierId").value = book.supplierId ?? "";
    $("#bookImageUrl").value = book.imageUrl ?? "";
    $("#bookFormTitle").textContent = `Editar libro #${book.id}`;
}

function resetBookForm() {
    $("#bookId").value = "";
    $("#bookTitle").value = "";
    $("#bookAuthor").value = "";
    $("#bookPrice").value = "";
    $("#bookStock").value = "";
    $("#bookSupplierId").value = "";
    $("#bookImageUrl").value = "";
    $("#bookFormTitle").textContent = "Nuevo libro";
    setText($("#bookFormError"), "");
}

async function handleBookFormSubmit(event) {
    event.preventDefault();
    const errorEl = $("#bookFormError");
    setText(errorEl, "");

    const id = $("#bookId").value ? parseInt($("#bookId").value, 10) : null;
    const dto = {
        title: $("#bookTitle").value.trim(),
        author: $("#bookAuthor").value.trim(),
        salePrice: parseFloat($("#bookPrice").value),
        currentStock: parseInt($("#bookStock").value, 10),
        supplierId: $("#bookSupplierId").value ? parseInt($("#bookSupplierId").value, 10) : null,
        imageUrl: $("#bookImageUrl").value.trim() || null,
    };

    try {
        if (id === null) {
            await apiRequest("/books/Create", {
                method: "POST",
                body: dto,
            });
        } else {
            await apiRequest(`/books/${id}/Update`, {
                method: "PUT",
                body: dto,
            });
        }

        resetBookForm();
        await loadBooks();
    } catch (err) {
        console.error(err);
        setText(errorEl, err.message || "Error al guardar el libro");
    }
}

async function loadCustomers() {
    const tbody = $("#customersTable tbody");
    tbody.innerHTML = "<tr><td colspan='6'>Cargando...</td></tr>";

    try {
        const res = await apiRequest("/customers/List");
        const customers = Array.isArray(res)
            ? res
            : Array.isArray(res?.data)
            ? res.data
            : [];

        if (!customers.length) {
            tbody.innerHTML = "<tr><td colspan='6'>No hay clientes registrados.</td></tr>";
            return;
        }

        tbody.innerHTML = "";
        for (const c of customers) {
            const tr = document.createElement("tr");
            const date = c.registeredAt ? new Date(c.registeredAt).toLocaleString() : "-";
            tr.innerHTML = `
                <td>${c.id}</td>
                <td>${c.name}</td>
                <td>${c.phone ?? "-"}</td>
                <td>${c.email ?? "-"}</td>
                <td>${date}</td>
                <td>
                    <button class="btn btn-small btn-outline" data-edit-customer="${c.id}">Editar</button>
                    <button class="btn btn-small btn-secondary" data-delete-customer="${c.id}">Borrar</button>
                </td>
            `;
            tbody.appendChild(tr);
        }

        tbody.querySelectorAll("[data-edit-customer]").forEach((btn) => {
            btn.addEventListener("click", () => {
                const id = parseInt(btn.dataset.editCustomer, 10);
                const c = customers.find((x) => x.id === id);
                if (!c) return;
                fillCustomerForm(c);
            });
        });

        tbody.querySelectorAll("[data-delete-customer]").forEach((btn) => {
            btn.addEventListener("click", async () => {
                const id = parseInt(btn.dataset.deleteCustomer, 10);
                if (!confirm(`¿Eliminar cliente #${id}?`)) return;
                try {
                    await apiRequest(`/customers/${id}/Delete`, { method: "DELETE" });
                    resetCustomerForm();
                    await loadCustomers();
                } catch (err) {
                    alert(err.message || "No se pudo eliminar el cliente");
                }
            });
        });
    } catch (err) {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan='6'>Error al cargar clientes: ${err.message}</td></tr>`;
    }
}

function fillCustomerForm(c) {
    $("#customerId").value = c.id;
    $("#customerName").value = c.name ?? "";
    $("#customerPhone").value = c.phone ?? "";
    $("#customerEmail").value = c.email ?? "";
    setText($("#customerFormError"), "");
}

function resetCustomerForm() {
    $("#customerId").value = "";
    $("#customerName").value = "";
    $("#customerPhone").value = "";
    $("#customerEmail").value = "";
    setText($("#customerFormError"), "");
}

async function handleCustomerFormSubmit(event) {
    event.preventDefault();
    const errorEl = $("#customerFormError");
    setText(errorEl, "");

    const id = $("#customerId").value ? parseInt($("#customerId").value, 10) : null;
    const dto = {
        name: $("#customerName").value.trim(),
        phone: $("#customerPhone").value.trim() || null,
        email: $("#customerEmail").value.trim() || null,
    };

    try {
        if (id === null) {
            await apiRequest("/customers/Create", {
                method: "POST",
                body: dto,
            });
        } else {
            await apiRequest(`/customers/${id}/Update`, {
                method: "PUT",
                body: dto,
            });
        }

        resetCustomerForm();
        await loadCustomers();
    } catch (err) {
        console.error(err);
        setText(errorEl, err.message || "Error al guardar el cliente");
    }
}

async function loadSuppliers() {
    const tbody = $("#suppliersTable tbody");
    tbody.innerHTML = "<tr><td colspan='5'>Cargando...</td></tr>";

    try {
        const response = await apiRequest("/suppliers/List");
        const suppliers = Array.isArray(response)
            ? response
            : Array.isArray(response?.data)
            ? response.data
            : [];

        if (!suppliers.length) {
            tbody.innerHTML = "<tr><td colspan='5'>No hay proveedores registrados o no tienes permiso.</td></tr>";
            return;
        }

        tbody.innerHTML = "";
        for (const s of suppliers) {
            const tr = document.createElement("tr");
            const date = s.associatedAt ? new Date(s.associatedAt).toLocaleDateString() : "-";
            tr.innerHTML = `
                <td>${s.id}</td>
                <td>${s.name}</td>
                <td>${s.contactInfo ?? "-"}</td>
                <td>${date}</td>
                <td>
                    <button class="btn btn-small btn-outline" data-edit-supplier="${s.id}">Editar</button>
                    <button class="btn btn-small btn-secondary" data-delete-supplier="${s.id}">Borrar</button>
                </td>
            `;
            tbody.appendChild(tr);
        }

        tbody.querySelectorAll("[data-edit-supplier]").forEach((btn) => {
            btn.addEventListener("click", () => {
                const id = parseInt(btn.dataset.editSupplier, 10);
                const s = suppliers.find((x) => x.id === id);
                if (!s) return;
                fillSupplierForm(s);
            });
        });

        tbody.querySelectorAll("[data-delete-supplier]").forEach((btn) => {
            btn.addEventListener("click", async () => {
                const id = parseInt(btn.dataset.deleteSupplier, 10);
                if (!confirm(`¿Eliminar proveedor #${id}?`)) return;
                try {
                    await apiRequest(`/suppliers/${id}/Delete`, { method: "DELETE" });
                    resetSupplierForm();
                    await loadSuppliers();
                } catch (err) {
                    alert(err.message || "No se pudo eliminar el proveedor");
                }
            });
        });
    } catch (err) {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan='5'>Error o sin permiso: ${err.message}</td></tr>`;
    }
}

function fillSupplierForm(s) {
    $("#supplierId").value = s.id;
    $("#supplierName").value = s.name ?? "";
    $("#supplierContactInfo").value = s.contactInfo ?? "";
    setText($("#supplierFormError"), "");
}

function resetSupplierForm() {
    $("#supplierId").value = "";
    $("#supplierName").value = "";
    $("#supplierContactInfo").value = "";
    setText($("#supplierFormError"), "");
}

async function handleSupplierFormSubmit(event) {
    event.preventDefault();
    const errorEl = $("#supplierFormError");
    setText(errorEl, "");

    const id = $("#supplierId").value ? parseInt($("#supplierId").value, 10) : null;
    const dto = {
        name: $("#supplierName").value.trim(),
        contactInfo: $("#supplierContactInfo").value.trim() || null,
    };

    try {
        if (id === null) {
            await apiRequest("/suppliers/Create", {
                method: "POST",
                body: dto,
            });
        } else {
            await apiRequest(`/suppliers/${id}/Update`, {
                method: "PUT",
                body: dto,
            });
        }

        resetSupplierForm();
        await loadSuppliers();
    } catch (err) {
        console.error(err);
        setText(errorEl, err.message || "Error al guardar proveedor");
    }
}

function addOrderItemRow(bookId = "", quantity = "") {
    const container = $("#orderItemsContainer");
    const row = document.createElement("div");
    row.className = "item-row";
    row.innerHTML = `
        <input type="number" min="1" step="1" class="form-input item-input order-book-id" placeholder="ID libro" value="${bookId}" />
        <input type="number" min="1" step="1" class="form-input item-input order-quantity" placeholder="Cantidad" value="${quantity}" />
        <button type="button" class="btn btn-small btn-secondary" data-remove-item>✕</button>
    `;
    container.appendChild(row);
}

function resetOrderForm() {
    $("#orderCustomerId").value = "";
    $("#orderItemsContainer").innerHTML = "";
    addOrderItemRow();
    setText($("#orderFormError"), "");
}

async function loadOrders() {
    const tbody = $("#ordersTable tbody");
    tbody.innerHTML = "<tr><td colspan='6'>Cargando...</td></tr>";

    try {
        const response = await apiRequest("/orders/List?PageNumber=1&PageSize=50");
        const page = response.data ?? response;
        const items = Array.isArray(page?.items) ? page.items : Array.isArray(page) ? page : [];

        if (!items.length) {
            tbody.innerHTML = "<tr><td colspan='6'>No hay pedidos registrados.</td></tr>";
            return;
        }

        tbody.innerHTML = "";
        for (const o of items) {
            const tr = document.createElement("tr");
            const date = o.orderDate ? new Date(o.orderDate).toLocaleString() : "-";
            const count = (o.items ?? []).length;
            tr.innerHTML = `
                <td>${o.id}</td>
                <td>${o.customerName ?? "-"}</td>
                <td>${date}</td>
                <td>${o.status}</td>
                <td>${count}</td>
                <td>
                    <button class="btn btn-small btn-secondary" data-cancel-order="${o.id}">Cancelar</button>
                </td>
            `;
            tbody.appendChild(tr);
        }

        tbody.querySelectorAll("[data-cancel-order]").forEach((btn) => {
            btn.addEventListener("click", async () => {
                const id = parseInt(btn.dataset.cancelOrder, 10);
                if (!confirm(`¿Cancelar pedido #${id}?`)) return;
                try {
                    await apiRequest(`/orders/${id}/cancel`, { method: "POST" });
                    await loadOrders();
                } catch (err) {
                    alert(err.message || "No se pudo cancelar el pedido");
                }
            });
        });
    } catch (err) {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan='6'>Error al cargar pedidos: ${err.message}</td></tr>`;
    }
}

async function handleOrderFormSubmit(event) {
    event.preventDefault();
    const errorEl = $("#orderFormError");
    setText(errorEl, "");

    const customerId = $("#orderCustomerId").value
        ? parseInt($("#orderCustomerId").value, 10)
        : null;

    const items = [];
    $$("#orderItemsContainer .item-row").forEach((row) => {
        const bookId = parseInt(row.querySelector(".order-book-id").value, 10);
        const quantity = parseInt(row.querySelector(".order-quantity").value, 10);
        if (bookId && quantity) {
            items.push({ bookId, quantity });
        }
    });

    if (!items.length) {
        setText(errorEl, "Agrega al menos un libro al pedido.");
        return;
    }

    const dto = { customerId, items };

    try {
        await apiRequest("/orders/Create", {
            method: "POST",
            body: dto,
        });
        resetOrderForm();
        await loadOrders();
    } catch (err) {
        console.error(err);
        setText(errorEl, err.message || "Error al crear el pedido");
    }
}

function handleOrderItemsClick(event) {
    const btn = event.target.closest("[data-remove-item]");
    if (btn) {
        const row = btn.closest(".item-row");
        if (!row) return;
        row.remove();
    }
}

function addSaleItemRow(bookId = "", quantity = "") {
    const container = $("#saleItemsContainer");
    const row = document.createElement("div");
    row.className = "item-row";
    row.innerHTML = `
        <input type="number" min="1" step="1" class="form-input item-input sale-book-id" placeholder="ID libro" value="${bookId}" />
        <input type="number" min="1" step="1" class="form-input item-input sale-quantity" placeholder="Cantidad" value="${quantity}" />
        <button type="button" class="btn btn-small btn-secondary" data-remove-sale-item>✕</button>
    `;
    container.appendChild(row);
}

function resetSaleForm() {
    $("#saleCustomerId").value = "";
    $("#saleOrderId").value = "";
    $("#saleItemsContainer").innerHTML = "";
    addSaleItemRow();
    setText($("#saleFormError"), "");
}

async function loadSales() {
    const tbody = $("#salesTable tbody");
    tbody.innerHTML = "<tr><td colspan='5'>Cargando...</td></tr>";

    try {
        const response = await apiRequest("/sales/List?PageNumber=1&PageSize=50");
        const page = response.data ?? response;
        const items = Array.isArray(page?.items) ? page.items : Array.isArray(page) ? page : [];

        if (!items.length) {
            tbody.innerHTML = "<tr><td colspan='5'>No hay ventas registradas.</td></tr>";
            return;
        }

        tbody.innerHTML = "";
        for (const s of items) {
            const tr = document.createElement("tr");
            const date = s.saleDate ? new Date(s.saleDate).toLocaleString() : "-";
            tr.innerHTML = `
                <td>${s.id}</td>
                <td>${s.customerName ?? "-"}</td>
                <td>${date}</td>
                <td>$${s.totalAmount.toFixed(2)}</td>
                <td>${(s.items ?? []).length}</td>
            `;
            tbody.appendChild(tr);
        }
    } catch (err) {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan='5'>Error al cargar ventas: ${err.message}</td></tr>`;
    }
}

async function createSaleRequest(dto) {
    const headers = { "Content-Type": "application/json" };
    if (authToken) {
        headers["Authorization"] = `Bearer ${authToken}`;
    }

    const response = await fetch(`${API_ORIGIN}/Create`, {
        method: "POST",
        headers,
        body: JSON.stringify(dto),
    });

    const contentType = response.headers.get("content-type") || "";
    let data = null;

    if (contentType.includes("application/json")) {
        data = await response.json();
    } else {
        const text = await response.text();
        if (!response.ok) {
            throw new Error(text || `Error HTTP ${response.status}`);
        }
        return null;
    }

    if (!response.ok || data.isSuccess === false) {
        const msg = data.message || data.Message || `Error HTTP ${response.status}`;
        throw new Error(msg);
    }

    return data; 
}

async function handleSaleFormSubmit(event) {
    event.preventDefault();
    const errorEl = $("#saleFormError");
    setText(errorEl, "");

    const customerId = $("#saleCustomerId").value
        ? parseInt($("#saleCustomerId").value, 10)
        : null;
    const orderId = $("#saleOrderId").value
        ? parseInt($("#saleOrderId").value, 10)
        : null;

    const items = [];
    $$("#saleItemsContainer .item-row").forEach((row) => {
        const bookId = parseInt(row.querySelector(".sale-book-id").value, 10);
        const quantity = parseInt(row.querySelector(".sale-quantity").value, 10);
        if (bookId && quantity) {
            items.push({ bookId, quantity });
        }
    });

    const hasOrder = !!orderId;
    const hasItems = items.length > 0;

    if (!hasOrder && !hasItems) {
        setText(
            errorEl,
            "Debes indicar un ID de pedido o agregar al menos un libro a la venta."
        );
        return;
    }

    let dtoItems;
    if (hasOrder && !hasItems) {
        dtoItems = [{ bookId: 1, quantity: 1 }];
    } else {
        dtoItems = items;
    }

    const dto = {
        customerId,
        orderId,
        items: dtoItems,
    };

    try {
        await createSaleRequest(dto); 
        resetSaleForm();
        await loadSales();
    } catch (err) {
        console.error(err);
        setText(errorEl, err.message || "Error al registrar la venta");
    }
}

function handleSaleItemsClick(event) {
    const btn = event.target.closest("[data-remove-sale-item]");
    if (btn) {
        const row = btn.closest(".item-row");
        if (!row) return;
        row.remove();
    }
}

async function loadAllData() {
    await Promise.all([
        loadBooks(),
        loadCustomers(),
        loadSuppliers(),
        loadOrders(),
        loadSales(),
    ]);
}

function init() {

    $("#loginTab").addEventListener("click", () => {
        $("#loginTab").classList.add("active");
        $("#registerTab").classList.remove("active");
        show($("#loginForm"));
        hide($("#registerForm"));
    });

    $("#registerTab").addEventListener("click", () => {
        $("#registerTab").classList.add("active");
        $("#loginTab").classList.remove("active");
        show($("#registerForm"));
        hide($("#loginForm"));
    });

    $("#loginForm").addEventListener("submit", handleLogin);
    $("#registerForm").addEventListener("submit", handleRegister);
    $("#logoutBtn").addEventListener("click", handleLogout);

    setupTabs();

    $("#refreshBooksBtn").addEventListener("click", loadBooks);
    $("#refreshCustomersBtn").addEventListener("click", loadCustomers);
    $("#refreshSuppliersBtn").addEventListener("click", loadSuppliers);
    $("#refreshOrdersBtn").addEventListener("click", loadOrders);
    $("#refreshSalesBtn").addEventListener("click", loadSales);

    $("#bookForm").addEventListener("submit", handleBookFormSubmit);
    $("#bookFormReset").addEventListener("click", resetBookForm);

    $("#customerForm").addEventListener("submit", handleCustomerFormSubmit);
    $("#customerFormReset").addEventListener("click", resetCustomerForm);

    $("#supplierForm").addEventListener("submit", handleSupplierFormSubmit);
    $("#supplierFormReset").addEventListener("click", resetSupplierForm);

    $("#orderForm").addEventListener("submit", handleOrderFormSubmit);
    $("#addOrderItemBtn").addEventListener("click", () => addOrderItemRow());
    $("#orderItemsContainer").addEventListener("click", handleOrderItemsClick);
    addOrderItemRow();

    $("#saleForm").addEventListener("submit", handleSaleFormSubmit);
    $("#addSaleItemBtn").addEventListener("click", () => addSaleItemRow());
    $("#saleItemsContainer").addEventListener("click", handleSaleItemsClick);
    addSaleItemRow();

    updateAuthUI(); 
}

document.addEventListener("DOMContentLoaded", init);