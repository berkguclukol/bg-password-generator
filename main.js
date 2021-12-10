const { app, BrowserWindow, ipcMain } = require("electron");
const path = require("path");
const ipc = ipcMain;
const createWindow = () => {
  const splash = new BrowserWindow({
    width: 300,
    height: 300,
    transparent: true,
    frame: false,
    alwaysOnTop: true,
  });
  splash.setIcon("icon.png");
  splash.loadFile("public/splash.html");
  splash.center();

  const win = new BrowserWindow({
    width: 600, // 600
    height: 550,
    maximizable: false,
    autoHideMenuBar: true,
    resizable: false,
    frame: false,
    show: false,
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false,
      preload: path.join(__dirname, "preload.js"),
    },
  });
  win.setIcon("icon.png");
  win.loadFile("public/index.html");

  setTimeout(function () {
    splash.close();
    win.show();
  }, 5000);
  ipc.on("app/minimize", () => {
    win.minimize();
  });
  ipc.on("app/maximize", () => {
    //
  });
  ipc.on("app/quit", () => {
    app.quit();
  });
};

app.whenReady().then(() => {
  createWindow();
  app.on("activate", () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});
app.on("window-all-closed", () => {
  if (process.platform !== "darwin") app.quit();
});
