/**
 * Air Quality Analytics & Sync - Frontend Application
 * Pure Vanilla JavaScript (ES6+) - Flat Monochrome Edition
 */

// Application State
const state = {
    apiBase: '',
    allRecords: [],
    currentPage: 1,
    pageSize: 50,
    currentMinRecord: null,
    currentMaxRecord: null,
};

// =============================================================================
// Initialization & Configuration
// =============================================================================
document.addEventListener('DOMContentLoaded', () => {
    initApiBase();
    initTabs();
    initEvents();
    checkApiHealth();
    loadAllData();
});

function initApiBase() {
    const input = document.getElementById('apiUrlInput');
    const isHosted = window.location.protocol.startsWith('http') && window.location.port === '5112';
    const defaultUrl = isHosted ? window.location.origin : 'http://localhost:5112';

    const savedUrl = localStorage.getItem('aqi_api_base');
    state.apiBase = savedUrl || defaultUrl;
    input.value = state.apiBase;

    input.addEventListener('change', () => {
        state.apiBase = input.value.trim().replace(/\/$/, '');
        localStorage.setItem('aqi_api_base', state.apiBase);
        showToast(`Збережено API URL: ${state.apiBase}`, 'info');
        checkApiHealth();
    });
}

function getApiUrl(path) {
    const cleanPath = path.startsWith('/') ? path : `/${path}`;
    if (!state.apiBase) return cleanPath;
    return `${state.apiBase}${cleanPath}`;
}

// =============================================================================
// API Health & Status
// =============================================================================
async function checkApiHealth() {
    const badge = document.getElementById('apiStatusBadge');
    badge.className = 'status-badge badge-neutral';
    badge.textContent = 'Перевірка...';

    try {
        const res = await fetch(getApiUrl('/api/v2/Records/analytics/count'), { method: 'GET' });
        if (res.ok) {
            badge.className = 'status-badge badge-dark';
            badge.textContent = 'Підключено';
        } else {
            badge.className = 'status-badge badge-outline';
            badge.textContent = `Помилка ${res.status}`;
        }
    } catch (err) {
        badge.className = 'status-badge badge-neutral';
        badge.textContent = 'Недоступний';
    }
}

// =============================================================================
// Tab Switching
// =============================================================================
function initTabs() {
    const tabBtns = document.querySelectorAll('.tab-btn');
    const tabContents = document.querySelectorAll('.tab-content');

    tabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            tabBtns.forEach(b => b.classList.remove('active'));
            tabContents.forEach(c => c.classList.remove('active'));

            btn.classList.add('active');
            const targetId = btn.getAttribute('data-tab');
            const targetTab = document.getElementById(targetId);
            if (targetTab) {
                targetTab.classList.add('active');
            }

            if (targetId === 'chartsTab') {
                loadAllCharts();
            }
        });
    });
}

// =============================================================================
// Event Listeners
// =============================================================================
function initEvents() {
    // Check API button
    document.getElementById('btnCheckApi').addEventListener('click', async () => {
        await checkApiHealth();
        await loadAllData();
    });

    // Records & Analytics Controls
    document.getElementById('btnRefreshRecords').addEventListener('click', loadAllData);
    document.getElementById('btnApplyFilter').addEventListener('click', applyFilters);
    document.getElementById('btnApplySort').addEventListener('click', applySort);
    document.getElementById('btnResetFilters').addEventListener('click', resetFilters);

    document.getElementById('btnFindById').addEventListener('click', findRecordById);
    document.getElementById('btnSearchCity').addEventListener('click', searchRecordByCity);

    // Pagination
    document.getElementById('btnPrevPage').addEventListener('click', () => {
        if (state.currentPage > 1) {
            state.currentPage--;
            renderRecordsTable();
        }
    });

    document.getElementById('btnNextPage').addEventListener('click', () => {
        const maxPage = Math.ceil(state.allRecords.length / state.pageSize) || 1;
        if (state.currentPage < maxPage) {
            state.currentPage++;
            renderRecordsTable();
        }
    });

    document.getElementById('pageSizeSelect').addEventListener('change', (e) => {
        state.pageSize = parseInt(e.target.value, 10);
        state.currentPage = 1;
        renderRecordsTable();
    });

    // Stat cards modal open
    document.getElementById('cardMinRecord').addEventListener('click', () => {
        if (state.currentMinRecord) openRecordModal(state.currentMinRecord);
    });
    document.getElementById('cardMaxRecord').addEventListener('click', () => {
        if (state.currentMaxRecord) openRecordModal(state.currentMaxRecord);
    });

    // Modal close
    document.getElementById('modalCloseBtn').addEventListener('click', closeRecordModal);
    document.getElementById('modalDismissBtn').addEventListener('click', closeRecordModal);
    document.getElementById('recordModal').addEventListener('click', (e) => {
        if (e.target.id === 'recordModal') closeRecordModal();
    });

    // File Inputs Label Update
    ['Csv', 'Json', 'Xml', 'Xlsx'].forEach(format => {
        const fileInput = document.getElementById(`file${format}`);
        const nameLabel = document.getElementById(`fileName${format}`);
        if (fileInput && nameLabel) {
            fileInput.addEventListener('change', () => {
                if (fileInput.files.length > 0) {
                    nameLabel.textContent = fileInput.files[0].name;
                } else {
                    nameLabel.textContent = `Виберіть файл .${format.toLowerCase()}`;
                }
            });
        }
    });

    // Import Buttons
    document.querySelectorAll('.btn-import').forEach(btn => {
        btn.addEventListener('click', () => {
            const format = btn.getAttribute('data-format');
            const inputId = btn.getAttribute('data-input');
            importData(format, inputId);
        });
    });

    // Export Buttons
    document.querySelectorAll('.btn-export').forEach(btn => {
        btn.addEventListener('click', () => {
            const format = btn.getAttribute('data-format');
            exportData(format);
        });
    });

    // Charts
    document.getElementById('btnReloadCharts').addEventListener('click', loadAllCharts);
    document.querySelectorAll('.btn-chart-download').forEach(btn => {
        btn.addEventListener('click', () => {
            const imgId = btn.getAttribute('data-img-id');
            downloadChartImage(imgId);
        });
    });

    // Reports
    document.getElementById('btnGenerateXlsx').addEventListener('click', downloadXlsxReport);
    document.getElementById('docxReportForm').addEventListener('submit', (e) => {
        e.preventDefault();
        downloadDocxReport();
    });

    // Folder Sync & Generator
    document.getElementById('btnAnalyzeSync').addEventListener('click', analyzeFolderSync);
    document.getElementById('btnExecuteSync').addEventListener('click', executeFolderSync);
    document.getElementById('testGenForm').addEventListener('submit', (e) => {
        e.preventDefault();
        generateTestFiles();
    });
}

// =============================================================================
// Tab 1: Records & Analytics
// =============================================================================
async function loadAllData() {
    await Promise.allSettled([
        loadAnalytics(),
        loadRecordsList()
    ]);
}

async function loadAnalytics() {
    try {
        const countRes = await fetch(getApiUrl('/api/v2/Records/analytics/count'));
        if (countRes.ok) {
            const count = await countRes.json();
            document.getElementById('statCount').textContent = count.toLocaleString();
        }

        const sumRes = await fetch(getApiUrl('/api/v2/Records/analytics/sum'));
        if (sumRes.ok) {
            const sum = await sumRes.json();
            document.getElementById('statSum').textContent = sum.toLocaleString(undefined, { minimumFractionDigits: 1 });
        }

        const medianRes = await fetch(getApiUrl('/api/v2/Records/analytics/median'));
        if (medianRes.ok) {
            const median = await medianRes.json();
            document.getElementById('statMedian').textContent = median.toLocaleString(undefined, { minimumFractionDigits: 1 });
        }

        const minRes = await fetch(getApiUrl('/api/v2/Records/analytics/min'));
        if (minRes.ok) {
            const minData = await minRes.json();
            state.currentMinRecord = minData;
            document.getElementById('statMinCity').textContent = minData.cityAndCountry || '—';
            document.getElementById('statMinValue').textContent = `PM2.5: ${minData.averagePollution} (Ранг: ${minData.rank})`;
        } else {
            document.getElementById('statMinCity').textContent = '—';
            document.getElementById('statMinValue').textContent = 'PM2.5: —';
        }

        const maxRes = await fetch(getApiUrl('/api/v2/Records/analytics/max'));
        if (maxRes.ok) {
            const maxData = await maxRes.json();
            state.currentMaxRecord = maxData;
            document.getElementById('statMaxCity').textContent = maxData.cityAndCountry || '—';
            document.getElementById('statMaxValue').textContent = `PM2.5: ${maxData.averagePollution} (Ранг: ${maxData.rank})`;
        } else {
            document.getElementById('statMaxCity').textContent = '—';
            document.getElementById('statMaxValue').textContent = 'PM2.5: —';
        }
    } catch (err) {
        console.error('Error loading analytics:', err);
    }
}

async function loadRecordsList() {
    try {
        const res = await fetch(getApiUrl('/api/v2/Records'));
        if (!res.ok) {
            throw new Error(`Статус: ${res.status}`);
        }
        const data = await res.json();
        state.allRecords = Array.isArray(data) ? data : [];
        state.currentPage = 1;
        renderRecordsTable();
    } catch (err) {
        console.error('Error fetching records:', err);
        renderEmptyTable('Помилка завантаження даних або бекенд офлайн.');
    }
}

async function applyFilters() {
    const city = document.getElementById('filterCity').value.trim();
    const minRank = document.getElementById('filterMinRank').value.trim();
    const maxRank = document.getElementById('filterMaxRank').value.trim();
    const minPollution = document.getElementById('filterMinPollution').value.trim();
    const maxPollution = document.getElementById('filterMaxPollution').value.trim();

    const params = new URLSearchParams();
    if (city) params.append('City', city);
    if (minRank) params.append('MinRank', minRank);
    if (maxRank) params.append('MaxRank', maxRank);
    if (minPollution) params.append('MinAveragePollution', minPollution);
    if (maxPollution) params.append('MaxAveragePollution', maxPollution);

    try {
        const res = await fetch(getApiUrl(`/api/v2/Records/filters?${params.toString()}`));
        if (!res.ok) throw new Error(`Статус ${res.status}`);
        const data = await res.json();
        state.allRecords = Array.isArray(data) ? data : [];
        state.currentPage = 1;
        renderRecordsTable();
        showToast(`Знайдено ${state.allRecords.length} записів за фільтром`, 'info');
    } catch (err) {
        showToast('Помилка при застосуванні фільтра', 'error');
    }
}

async function applySort() {
    const sortBy = document.getElementById('sortBy').value;
    const direction = document.getElementById('sortDirection').value;

    try {
        const res = await fetch(getApiUrl(`/api/v2/Records/sort?sortBy=${sortBy}&direction=${direction}`));
        if (!res.ok) throw new Error(`Статус ${res.status}`);
        const data = await res.json();
        state.allRecords = Array.isArray(data) ? data : [];
        state.currentPage = 1;
        renderRecordsTable();
        showToast(`Відсортовано за ${sortBy} (${direction})`, 'info');
    } catch (err) {
        showToast('Помилка сортування', 'error');
    }
}

function resetFilters() {
    document.getElementById('filterForm').reset();
    loadRecordsList();
    showToast('Фільтри та сортування скинуто', 'info');
}

async function findRecordById() {
    const idInput = document.getElementById('inputFindId');
    const id = idInput.value.trim();
    if (!id) {
        showToast('Введіть ID запису', 'warning');
        return;
    }

    try {
        const res = await fetch(getApiUrl(`/api/v2/Records/${id}`));
        if (res.status === 404) {
            showToast(`Запис з ID ${id} не знайдено`, 'error');
            return;
        }
        if (!res.ok) throw new Error(`Статус ${res.status}`);
        const record = await res.json();
        openRecordModal(record);
    } catch (err) {
        showToast(`Помилка пошуку ID: ${err.message}`, 'error');
    }
}

async function searchRecordByCity() {
    const cityInput = document.getElementById('inputSearchCity');
    const city = cityInput.value.trim();
    if (!city) {
        showToast('Введіть назву міста для пошуку', 'warning');
        return;
    }

    try {
        const res = await fetch(getApiUrl(`/api/v2/Records/search?city=${encodeURIComponent(city)}`));
        if (res.status === 404) {
            showToast(`Місто '${city}' не знайдено`, 'error');
            return;
        }
        if (!res.ok) throw new Error(`Статус ${res.status}`);
        const record = await res.json();
        openRecordModal(record);
    } catch (err) {
        showToast(`Помилка пошуку за містом: ${err.message}`, 'error');
    }
}

// Render Wide Table with Pagination
function renderRecordsTable() {
    const tbody = document.getElementById('recordsTableBody');
    const totalRecords = state.allRecords.length;
    document.getElementById('recordsCountBadge').textContent = `${totalRecords.toLocaleString()} записів`;

    if (totalRecords === 0) {
        renderEmptyTable('Записів не знайдено. Будь ласка, виконайте імпорт файлу.');
        updatePaginationControls(0, 0);
        return;
    }

    const totalPages = Math.ceil(totalRecords / state.pageSize) || 1;
    if (state.currentPage > totalPages) state.currentPage = totalPages;

    const startIdx = (state.currentPage - 1) * state.pageSize;
    const pageItems = state.allRecords.slice(startIdx, startIdx + state.pageSize);

    tbody.innerHTML = '';
    pageItems.forEach(r => {
        const m = r.measurements || {};
        const tr = document.createElement('tr');
        tr.innerHTML = `
      <td>${r.id}</td>
      <td><strong>#${r.rank}</strong></td>
      <td><strong>${escapeHtml(r.cityAndCountry)}</strong></td>
      <td><strong>${r.averagePollution}</strong></td>
      <td>${getAqiCategoryBadge(r.averagePollution)}</td>
      <td>${formatMonthVal(m.january)}</td>
      <td>${formatMonthVal(m.february)}</td>
      <td>${formatMonthVal(m.march)}</td>
      <td>${formatMonthVal(m.april)}</td>
      <td>${formatMonthVal(m.may)}</td>
      <td>${formatMonthVal(m.june)}</td>
      <td>${formatMonthVal(m.july)}</td>
      <td>${formatMonthVal(m.august)}</td>
      <td>${formatMonthVal(m.september)}</td>
      <td>${formatMonthVal(m.october)}</td>
      <td>${formatMonthVal(m.november)}</td>
      <td>${formatMonthVal(m.december)}</td>
      <td>
        <button class="btn btn-sm btn-secondary btn-view-row" data-id="${r.id}">
          Інфо
        </button>
      </td>
    `;

        tr.querySelector('.btn-view-row').addEventListener('click', () => {
            openRecordModal(r);
        });

        tbody.appendChild(tr);
    });

    updatePaginationControls(state.currentPage, totalPages);
}

function updatePaginationControls(page, totalPages) {
    const btnPrev = document.getElementById('btnPrevPage');
    const btnNext = document.getElementById('btnNextPage');
    const indicator = document.getElementById('pageIndicator');

    indicator.textContent = `Сторінка ${page || 0} / ${totalPages || 0}`;
    btnPrev.disabled = page <= 1;
    btnNext.disabled = page >= totalPages;
}

function renderEmptyTable(message) {
    const tbody = document.getElementById('recordsTableBody');
    tbody.innerHTML = `
    <tr>
      <td colspan="18" class="text-center empty-state">${escapeHtml(message)}</td>
    </tr>
  `;
}

function formatMonthVal(val) {
    if (val === null || val === undefined) return '<span style="color:#a3a3a3">—</span>';
    return val.toFixed(1);
}

function getAqiCategoryBadge(pollution) {
    if (pollution <= 50) return '<span class="badge badge-neutral">Добра (&le;50)</span>';
    if (pollution <= 100) return '<span class="badge badge-neutral">Помірна (51-100)</span>';
    if (pollution <= 150) return '<span class="badge badge-outline">Шкідл. чутл. (101-150)</span>';
    if (pollution <= 200) return '<span class="badge badge-outline">Шкідлива (151-200)</span>';
    if (pollution <= 300) return '<span class="badge badge-dark">Дуже шкідл. (201-300)</span>';
    return '<span class="badge badge-dark">Небезпечна (&gt;300)</span>';
}

// =============================================================================
// Record Details Modal
// =============================================================================
function openRecordModal(record) {
    const modal = document.getElementById('recordModal');
    const title = document.getElementById('modalTitle');
    const body = document.getElementById('modalBody');
    const m = record.measurements || {};

    title.textContent = `Запис #${record.id}: ${record.cityAndCountry}`;

    body.innerHTML = `
    <div class="detail-grid">
      <div class="detail-item">
        <div class="detail-label">Ранг</div>
        <div class="detail-value">#${record.rank}</div>
      </div>
      <div class="detail-item">
        <div class="detail-label">Середній PM2.5</div>
        <div class="detail-value">${record.averagePollution} мкг/м³</div>
      </div>
      <div class="detail-item">
        <div class="detail-label">Категорія якості</div>
        <div style="margin-top:4px;">${getAqiCategoryBadge(record.averagePollution)}</div>
      </div>
    </div>

    <h4 style="margin: 14px 0 6px 0; font-size: 0.85rem; font-weight: 700; text-transform: uppercase;">Помісячні вимірювання (PM2.5):</h4>
    <div class="months-bar-container">
      <div class="month-cell"><div class="month-name">Січ</div><div class="month-val">${m.january ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Лют</div><div class="month-val">${m.february ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Бер</div><div class="month-val">${m.march ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Кві</div><div class="month-val">${m.april ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Тра</div><div class="month-val">${m.may ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Чер</div><div class="month-val">${m.june ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Лип</div><div class="month-val">${m.july ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Сер</div><div class="month-val">${m.august ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Вер</div><div class="month-val">${m.september ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Жов</div><div class="month-val">${m.october ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Лис</div><div class="month-val">${m.november ?? '—'}</div></div>
      <div class="month-cell"><div class="month-name">Гру</div><div class="month-val">${m.december ?? '—'}</div></div>
    </div>
  `;

    modal.classList.remove('hidden');
}

function closeRecordModal() {
    document.getElementById('recordModal').classList.add('hidden');
}

// =============================================================================
// Tab 2: Import & Export
// =============================================================================
async function importData(format, inputId) {
    const fileInput = document.getElementById(inputId);
    if (!fileInput.files || fileInput.files.length === 0) {
        showToast(`Виберіть файл для імпорту (${format})`, 'warning');
        return;
    }

    const file = fileInput.files[0];
    const formData = new FormData();
    formData.append('file', file);

    const logBox = document.getElementById('importStatusLog');
    logBox.textContent = `[${new Date().toLocaleTimeString()}] Початок імпорту ${file.name} (${format})...\n`;

    try {
        const res = await fetch(getApiUrl(`/api/v2/${format}/import`), {
            method: 'POST',
            body: formData,
        });

        if (!res.ok) {
            const errText = await res.text();
            throw new Error(`Сервер повернув ${res.status}: ${errText}`);
        }

        const data = await res.json();
        const count = Array.isArray(data) ? data.length : 0;

        logBox.textContent += `[${new Date().toLocaleTimeString()}] Успішно! Імпортовано ${count} записів з ${file.name}.\n`;
        showToast(`Успішно імпортовано ${count} записів (${format})`, 'success');

        await loadAllData();
    } catch (err) {
        logBox.textContent += `[${new Date().toLocaleTimeString()}] Помилка імпорту: ${err.message}\n`;
        showToast(`Помилка імпорту: ${err.message}`, 'error');
    }
}

async function exportData(format) {
    try {
        showToast(`Генерація ${format} файлу...`, 'info');
        const res = await fetch(getApiUrl(`/api/v2/${format}/export`));
        if (!res.ok) throw new Error(`Помилка ${res.status}: ${await res.text()}`);

        const blob = await res.blob();
        const ext = format.toLowerCase();
        const filename = `air_quality.${ext}`;

        triggerDownload(blob, filename);
        showToast(`Файл ${filename} успішно завантажено`, 'success');
    } catch (err) {
        showToast(`Помилка експорту: ${err.message}`, 'error');
    }
}

// =============================================================================
// Tab 3: Charts
// =============================================================================
async function loadAllCharts() {
    const city = document.getElementById('chartFilterCity').value.trim();
    const minRank = document.getElementById('chartMinRank').value.trim();
    const maxRank = document.getElementById('chartMaxRank').value.trim();
    const minPollution = document.getElementById('chartMinPollution').value.trim();
    const maxPollution = document.getElementById('chartMaxPollution').value.trim();

    const params = new URLSearchParams();
    if (city) params.append('City', city);
    if (minRank) params.append('MinRank', minRank);
    if (maxRank) params.append('MaxRank', maxRank);
    if (minPollution) params.append('MinAveragePollution', minPollution);
    if (maxPollution) params.append('MaxAveragePollution', maxPollution);

    const query = params.toString() ? `?${params.toString()}` : '';

    loadChartImage('/api/v2/Charts/top-polluted' + query, 'imgChartTop', 'loaderChartTop');
    loadChartImage('/api/v2/Charts/distribution' + query, 'imgChartDist', 'loaderChartDist');
    loadChartImage('/api/v2/Charts/monthly-trend' + query, 'imgChartTrend', 'loaderChartTrend');
}

async function loadChartImage(endpoint, imgId, loaderId) {
    const img = document.getElementById(imgId);
    const loader = document.getElementById(loaderId);
    loader.style.display = 'block';
    loader.textContent = 'Генерація графіка...';
    img.style.display = 'none';

    try {
        const res = await fetch(getApiUrl(endpoint));
        if (res.status === 204) {
            loader.textContent = 'Немає даних для побудови графіка (імпортуйте файл).';
            return;
        }
        if (!res.ok) {
            loader.textContent = `Помилка отримання графіка (${res.status})`;
            return;
        }

        const blob = await res.blob();
        const objectUrl = URL.createObjectURL(blob);
        img.src = objectUrl;
        img.onload = () => {
            loader.style.display = 'none';
            img.style.display = 'block';
        };
    } catch (err) {
        loader.textContent = 'Помилка підключення до сервера';
    }
}

function downloadChartImage(imgId) {
    const img = document.getElementById(imgId);
    if (!img.src || img.style.display === 'none') {
        showToast('Графік ще не завантажений', 'warning');
        return;
    }

    const a = document.createElement('a');
    a.href = img.src;
    a.download = `${imgId}.png`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    showToast('Графік збережено як PNG', 'success');
}

// =============================================================================
// Tab 4: Reports
// =============================================================================
async function downloadXlsxReport() {
    try {
        showToast('Формування Excel-звіту...', 'info');
        const res = await fetch(getApiUrl('/api/v2/Reports/xlsx'));
        if (!res.ok) throw new Error(`Помилка ${res.status}: ${await res.text()}`);

        const blob = await res.blob();
        triggerDownload(blob, 'air-quality-report.xlsx');
        showToast('Excel-звіт успішно сформовано', 'success');
    } catch (err) {
        showToast(`Помилка генерації Excel: ${err.message}`, 'error');
    }
}

async function downloadDocxReport() {
    const studentName = document.getElementById('reportStudentName').value.trim();
    const group = document.getElementById('reportGroup').value.trim();
    const variant = document.getElementById('reportVariant').value.trim();
    const datasetDescription = document.getElementById('reportDescription').value.trim();

    const payload = {
        studentName,
        group,
        variant,
        datasetDescription
    };

    try {
        showToast('Формування DOCX звіту...', 'info');
        const res = await fetch(getApiUrl('/api/v2/Reports/docx'), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!res.ok) throw new Error(`Помилка ${res.status}: ${await res.text()}`);

        const blob = await res.blob();
        triggerDownload(blob, 'air-quality-report.docx');
        showToast('DOCX-звіт успішно згенеровано', 'success');
    } catch (err) {
        showToast(`Помилка генерації DOCX: ${err.message}`, 'error');
    }
}

// =============================================================================
// Tab 5: Folder Sync & Generator
// =============================================================================
async function generateTestFiles() {
    const folderCount = document.getElementById('genFolderCount').value || 5;
    const filesPerFolder = document.getElementById('genFilesPerFolder').value || 5;

    try {
        showToast('Генерація тестових файлів...', 'info');
        const res = await fetch(getApiUrl(`/api/v1/test-files/generate?folderCount=${folderCount}&filesPerFolder=${filesPerFolder}`), {
            method: 'POST'
        });

        if (!res.ok) throw new Error(`Помилка ${res.status}: ${await res.text()}`);

        const data = await res.json();
        const resultBox = document.getElementById('testGenResult');
        const details = document.getElementById('testGenDetails');

        resultBox.classList.remove('hidden');
        details.innerHTML = `
      Каталог: ${escapeHtml(data.rootPath)}<br>
      Папок: ${data.folderCount}<br>
      Файлів: ${data.createdFiles?.length || 0}
    `;

        if (data.rootPath) {
            document.getElementById('syncSource').value = data.rootPath;
            document.getElementById('syncTarget').value = `${data.rootPath}_Backup`;
        }

        showToast('Тестові файли згенеровано', 'success');
    } catch (err) {
        showToast(`Помилка генерації файлів: ${err.message}`, 'error');
    }
}

async function analyzeFolderSync() {
    const sourceDirectory = document.getElementById('syncSource').value.trim();
    const targetDirectory = document.getElementById('syncTarget').value.trim();
    const direction = document.getElementById('syncDirection').value;

    if (!sourceDirectory || !targetDirectory) {
        showToast('Вкажіть обидва каталоги (Source і Target)', 'warning');
        return;
    }

    try {
        showToast('Аналіз різниці каталогів...', 'info');
        const res = await fetch(getApiUrl('/api/v1/folder-sync/analyze'), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ sourceDirectory, targetDirectory, direction })
        });

        if (!res.ok) throw new Error(`Помилка ${res.status}: ${await res.text()}`);

        const items = await res.json();
        renderSyncResults(items, 'Аналіз завершено', 'badge-neutral');
        showToast(`Виявлено ${items.length} змін`, 'info');
    } catch (err) {
        showToast(`Помилка аналізу: ${err.message}`, 'error');
    }
}

async function executeFolderSync() {
    const sourceDirectory = document.getElementById('syncSource').value.trim();
    const targetDirectory = document.getElementById('syncTarget').value.trim();
    const direction = document.getElementById('syncDirection').value;

    if (!sourceDirectory || !targetDirectory) {
        showToast('Вкажіть обидва каталоги (Source і Target)', 'warning');
        return;
    }

    try {
        showToast('Синхронізація...', 'info');
        const res = await fetch(getApiUrl('/api/v1/folder-sync/execute'), {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ sourceDirectory, targetDirectory, direction })
        });

        if (!res.ok) throw new Error(`Помилка ${res.status}: ${await res.text()}`);

        const result = await res.json();
        renderSyncResults(result.items || [], 'Синхронізовано', 'badge-dark');
        document.getElementById('syncSummaryText').textContent =
            `Синхронізовано: ${result.syncedFiles} з ${result.totalAnalyzed}. ${result.message}`;
        showToast(`Синхронізовано файлів: ${result.syncedFiles}`, 'success');
    } catch (err) {
        showToast(`Помилка синхронізації: ${err.message}`, 'error');
    }
}

function renderSyncResults(items, statusText, statusBadgeClass) {
    const tbody = document.getElementById('syncTableBody');
    const badge = document.getElementById('syncStatusBadge');
    badge.className = `badge ${statusBadgeClass}`;
    badge.textContent = `${statusText} (${items.length})`;

    if (!items || items.length === 0) {
        tbody.innerHTML = `
      <tr>
        <td colspan="5" class="text-center empty-state">Змін не виявлено (каталоги ідентичні).</td>
      </tr>
    `;
        return;
    }

    tbody.innerHTML = '';
    items.forEach(item => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
      <td>${getChangeTypeBadge(item.changeType)}</td>
      <td><code>${escapeHtml(item.relativePath)}</code></td>
      <td>${escapeHtml(item.description)}</td>
      <td title="${escapeHtml(item.sourceFullPath || '')}"><small>${escapeHtml(item.sourceFullPath || '—')}</small></td>
      <td title="${escapeHtml(item.targetFullPath || '')}"><small>${escapeHtml(item.targetFullPath || '—')}</small></td>
    `;
        tbody.appendChild(tr);
    });
}

function getChangeTypeBadge(type) {
    if (type === 'New' || type === 0) {
        return '<span class="badge badge-outline">Новий (New)</span>';
    }
    if (type === 'Modified' || type === 1) {
        return '<span class="badge badge-neutral">Змінений (Modified)</span>';
    }
    if (type === 'Deleted' || type === 2) {
        return '<span class="badge badge-dark">Видалений (Deleted)</span>';
    }
    return `<span class="badge badge-neutral">${type}</span>`;
}

// =============================================================================
// Helper Functions
// =============================================================================
function triggerDownload(blob, filename) {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

function showToast(message, type = 'info') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = 'toast';

    let prefix = '[ІНФО]';
    if (type === 'success') prefix = '[OK]';
    if (type === 'error') prefix = '[ПОМИЛКА]';
    if (type === 'warning') prefix = '[УВАГА]';

    toast.textContent = `${prefix} ${message}`;
    container.appendChild(toast);

    setTimeout(() => {
        toast.remove();
    }, 3500);
}

function escapeHtml(str) {
    if (!str) return '';
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}
