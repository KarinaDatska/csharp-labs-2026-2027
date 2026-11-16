const get = id => document.getElementById(id);
const api = location.protocol === 'file:' ? 'http://localhost:5113' : '';

async function synchronize(execute) {
    if (!get('syncForm').reportValidity()) return;
    const config = { sourceDirectory: get('source').value.trim(),
        targetDirectory: get('target').value.trim(), direction: get('direction').value };
    if (execute && !confirm('Застосувати зміни? Файли цільової папки можуть бути замінені або видалені.')) return;
    const buttons = document.querySelectorAll('button');
    buttons.forEach(button => button.disabled = true);
    get('status').className = '';
    get('status').textContent = 'Виконується…';
    try {
        const response = await fetch(api + '/api/v1/folder-sync/' + (execute ? 'execute' : 'analyze'), {
            method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(config) });
        const result = await response.json();
        if (!response.ok) throw new Error(result.message || result.title || 'Помилка запиту');
        const items = execute ? result.items : result;
        get('changes').replaceChildren();
        const names = { New: 'Новий', Modified: 'Змінений', Deleted: 'Видалений' };
        for (const item of items) {
            const row = document.createElement('tr');
            for (const value of [names[item.changeType], item.relativePath, item.sourceFullPath, item.targetFullPath]) {
                const cell = document.createElement('td');
                cell.textContent = value;
                row.append(cell);
            }
            get('changes').append(row);
        }
        get('status').textContent = execute ? result.message : `Змін: ${items.length}`;
    } catch (error) {
        get('status').className = 'error';
        get('status').textContent = error.message;
    } finally {
        buttons.forEach(button => button.disabled = false);
    }
}

get('syncForm').onsubmit = event => { event.preventDefault(); synchronize(false); };
get('execute').onclick = () => synchronize(true);
