const get = id => document.getElementById(id);
const api = location.protocol === 'file:' ? 'http://localhost:5112' : '';
const months = ['january', 'february', 'march', 'april', 'may', 'june', 'july', 'august', 'september', 'october', 'november', 'december'];
const monthNames = ['Січ', 'Лют', 'Бер', 'Кві', 'Тра', 'Чер', 'Лип', 'Сер', 'Вер', 'Жов', 'Лис', 'Гру'];
const headings = ['ID', 'Ранг', 'Місто, країна', 'Середнє', ...monthNames];
let page = 1;
let total = 0;
let fileName = '';
let chartUrl = '';

async function request(path, settings = {}) {
    const response = await fetch(api + path, settings);
    if (!response.ok) {
        const error = await response.json().catch(() => ({}));
        throw new Error(error.message || error.title || `Помилка ${response.status}`);
    }
    return response;
}

async function run(action) {
    get('message').className = '';
    get('message').textContent = 'Виконується…';
    try {
        await action();
        get('message').textContent = 'Готово';
    } catch (error) {
        get('message').className = 'error';
        get('message').textContent = error.message;
    }
}

function filters() {
    const query = new URLSearchParams();
    for (const id of ['city', 'minRank', 'maxRank', 'minAveragePollution', 'maxAveragePollution'])
        if (get(id).value.trim()) query.set(id, get(id).value.trim());
    return query;
}

function table(container, headers, rows) {
    const element = document.createElement('table');
    const head = element.createTHead().insertRow();
    for (const title of headers) {
        const cell = document.createElement('th');
        cell.textContent = title;
        head.append(cell);
    }
    const body = element.createTBody();
    if (!rows.length) {
        const cell = body.insertRow().insertCell();
        cell.colSpan = headers.length;
        cell.textContent = 'Записів немає. Імпортуйте дані або змініть фільтри.';
    }
    for (const values of rows) {
        const row = body.insertRow();
        for (const value of values) {
            const cell = row.insertCell();
            if (value instanceof Node) cell.append(value);
            else cell.textContent = value == null ? '—' : value;
        }
    }
    container.replaceChildren(element);
}

function values(record) {
    return [record.id, record.rank, record.cityAndCountry, record.averagePollution,
        ...months.map(month => record.measurements[month])];
}

function button(title, action) {
    const element = document.createElement('button');
    element.textContent = title;
    element.onclick = action;
    return element;
}

async function loadData() {
    const query = filters();
    query.set('sortBy', get('sortBy').value);
    query.set('descending', get('descending').value);
    query.set('page', page);
    const data = await (await request('/api/data?' + query)).json();
    total = data.total;
    const rows = data.records.map(record => [...values(record),
        button('Змінити', () => edit(record)), button('Видалити', () => run(() => remove(record.id)))]);
    table(get('recordsTable'), [...headings, '', ''], rows);
    table(get('groups'), ['Країна', 'Кількість', 'Сума', 'Середнє', 'Мін.', 'Макс.'],
        data.groups.map(group => [group.country, group.count, group.sum.toFixed(2), group.average.toFixed(2), group.min, group.max]));
    const summary = data.summary;
    get('summary').textContent = `Записів: ${total} · Середнє: ${summary.averagePollutionMean} · Медіана: ${summary.medianPollution} · Мін.: ${summary.minPollution} · Макс.: ${summary.maxPollution}`;
    get('page').textContent = `${page} / ${Math.max(1, Math.ceil(total / 50))}`;
    get('previous').disabled = page === 1;
    get('next').disabled = page * 50 >= total;
    get('status').textContent = `${fileName || 'Файл не відкрито'} · ${total} записів за фільтром · ${new Date().toLocaleTimeString()}`;
}

async function importFile() {
    const file = get('file').files[0];
    if (!file) throw new Error('Оберіть файл.');
    const form = new FormData();
    form.append('file', file);
    const data = await (await request('/api/files/import', { method: 'POST', body: form })).json();
    fileName = data.name;
    get('filters').reset();
    page = 1;
    await loadData();
    tab('records');
}

function edit(record) {
    get('editForm').reset();
    get('recordId').value = record?.id || '';
    get('editRank').value = record?.rank || 1;
    get('editCity').value = record?.cityAndCountry || '';
    get('editAverage').value = record?.averagePollution ?? '';
    for (const month of months) get(month).value = record?.measurements[month] ?? '';
    get('editor').showModal();
}

async function save() {
    const id = get('recordId').value;
    const record = { rank: Number(get('editRank').value), cityAndCountry: get('editCity').value,
        averagePollution: Number(get('editAverage').value), measurements: {} };
    for (const month of months) record.measurements[month] = get(month).value === '' ? null : Number(get(month).value);
    await request('/api/data' + (id ? '/' + id : ''), { method: id ? 'PUT' : 'POST',
        headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(record) });
    get('editor').close();
    await loadData();
}

async function remove(id) {
    if (!confirm('Видалити запис?')) return;
    await request('/api/data/' + id, { method: 'DELETE' });
    if (total % 50 === 1 && page > 1) page--;
    await loadData();
}

async function download(path, name, settings) {
    const blob = await (await request(path, settings)).blob();
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = name;
    link.click();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
}

async function draw() {
    const response = await request(chartPath());
    if (response.status === 204) throw new Error('Немає даних для графіка.');
    if (chartUrl) URL.revokeObjectURL(chartUrl);
    chartUrl = URL.createObjectURL(await response.blob());
    get('chart').src = chartUrl;
    get('chart').hidden = false;
}

function chartPath() {
    const query = filters();
    query.set('valueField', get('chartField').value);
    return '/api/v2/Charts/' + get('chartType').value + '?' + query;
}

async function docxReport(metadata) {
    const screenshot = get('screenshot').files[0];
    if (screenshot) {
        if (screenshot.size > 5000000) throw new Error('PNG має бути до 5 МБ.');
        metadata.screenshotPng = await new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = () => resolve(reader.result.split(',')[1]);
            reader.onerror = () => reject(new Error('Не вдалося прочитати PNG.'));
            reader.readAsDataURL(screenshot);
        });
    }
    await download('/api/v2/Reports/docx', 'report.docx', { method: 'POST',
        headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(metadata) });
}

function tab(id) {
    for (const section of document.querySelectorAll('body > section')) section.hidden = section.id !== id;
    for (const item of document.querySelectorAll('[data-tab]')) item.setAttribute('aria-selected', String(item.dataset.tab === id));
}

for (const element of document.querySelectorAll('[data-tab]')) element.onclick = () => tab(element.dataset.tab);
for (let index = 0; index < months.length; index++) {
    get('chartField').add(new Option(monthNames[index], months[index]));
    const label = document.createElement('label');
    label.textContent = monthNames[index];
    const input = document.createElement('input');
    input.id = months[index]; input.type = 'number'; input.min = '0'; input.step = 'any';
    label.append(input); get('months').append(label);
}
get('file').onchange = () => { get('import').disabled = !get('file').files.length; };
get('import').onclick = () => run(importFile);
get('export').onclick = () => run(() => download('/api/files/export/' + get('format').value, 'air_quality.' + get('format').value));
get('filters').onsubmit = event => {
    event.preventDefault();
    for (const [low, high] of [['minRank', 'maxRank'], ['minAveragePollution', 'maxAveragePollution']]) {
        if (get(low).value && get(high).value && Number(get(low).value) > Number(get(high).value)) {
            get('message').className = 'error';
            get('message').textContent = 'Мінімальне значення не може перевищувати максимальне.';
            get(low).focus();
            return;
        }
    }
    page = 1; run(loadData);
};
get('filters').onreset = () => { page = 1; setTimeout(() => run(loadData), 0); };
get('previous').onclick = () => { page--; run(loadData); };
get('next').onclick = () => { page++; run(loadData); };
get('add').onclick = () => edit(null);
get('editForm').onsubmit = event => { event.preventDefault(); run(save); };
get('cancelEdit').onclick = () => get('editor').close();
get('draw').onclick = () => run(draw);
get('chartType').onchange = () => {
    get('chartField').disabled = get('chartType').value === 'monthly-trend';
    run(draw);
};
get('chartField').onchange = () => run(draw);
get('saveChart').onclick = () => run(() => download(chartPath(), get('chartType').value + '.png'));
get('xlsxReport').onclick = () => run(() => download('/api/v2/Reports/xlsx', 'report.xlsx'));
get('reportForm').onsubmit = event => {
    event.preventDefault();
    const metadata = { studentName: get('student').value, group: get('group').value,
        variant: get('variant').value, datasetDescription: get('description').value };
    run(() => docxReport(metadata));
};
get('about').onclick = () => alert('Якість повітря — імпорт, редагування, аналіз, графіки та звіти. Дацька Карина, КН-21.');
tab('files');
run(loadData);
