function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("orderId", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    const orderIdRaw = ev.dataTransfer.getData("orderId");
    const orderElement = document.getElementById(orderIdRaw);
    const newStatus = ev.target.closest(".col-md-4").id;

    if (!orderElement || !newStatus) return;


    const targetContainer = document.querySelector(`#${newStatus} > div`);
    targetContainer.appendChild(orderElement);


    const orderId = parseInt(orderIdRaw.replace("order-", ""));
    const status = statusFromColumnId(newStatus);


    fetch('/Order/UpdateStatus', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `orderID=${orderId}&status=${status}`
    })
        .then(res => res.json())
        .then(data => {
            if (!data.success) {
                alert("Något gick fel vid uppdatering.");
                console.error(data.message);
            }
            else {
                location.reload();
            }
        })
        .catch(error => {
            console.error("Fel vid fetch:", error);
            alert("Fel vid uppdatering.");
        });
}

function statusFromColumnId(columnId) {
    switch (columnId) {
        case "ejBehandlad": return "Ej behandlad";
        case "paborjad": return "Påbörjad";
        case "avslutad": return "Avslutad";
        default: return "";
    }
}
