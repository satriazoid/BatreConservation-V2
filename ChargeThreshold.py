import os
import subprocess
import tkinter as tk
from tkinter import ttk, messagebox

EXE_PATH = r"C:\System\App\ChargeThreshold.exe"
# Path lengkap menuju executable Anda
# EXE_PATH = r"C:\System\Code\pribadi-project\Apps\BatreConservation-V2\ChargeThreshold.exe"

def run_exe_command(args):
    if not os.path.exists(EXE_PATH):
        messagebox.showerror("Error", f"Executable tidak ditemukan di:\n{EXE_PATH}")
        return None

    try:
        CREATE_NO_WINDOW = 0x08000000
        cmd = [EXE_PATH] + args
        
        process = subprocess.Popen(
            cmd,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            creationflags=CREATE_NO_WINDOW
        )
        
        stdout, stderr = process.communicate()
        return stdout.strip() if stdout else stderr.strip()

    except Exception as e:
        messagebox.showerror("Gagal Eksekusi", f"Terjadi kesalahan:\n{e}")
        return None

def update_status_label(text, is_error=False):
    lbl_status_val.config(text=text, foreground="#d13438" if is_error else "#107c41")

def set_threshold_on():
    max_val = int(scale_max.get())
    min_val = int(scale_min.get())
    
    if min_val >= max_val:
        messagebox.showwarning("Peringatan", "Batas Bawah harus lebih kecil dari Batas Atas!")
        return

    result = run_exe_command(["on", str(max_val), str(min_val)])
    if result:
        update_status_label(result)

def set_threshold_off():
    result = run_exe_command(["off"])
    if result:
        update_status_label(result)

def check_status():
    result = run_exe_command(["status"])
    if result:
        update_status_label(result)

def on_slider_change(val):
    lbl_max_val.config(text=f"{int(scale_max.get())}%")
    lbl_min_val.config(text=f"{int(scale_min.get())}%")


root = tk.Tk()
root.title("Charge Threshold Manager")
root.geometry("400x440")
root.resizable(False, False)

style = ttk.Style(root)
style.theme_use("vista")

style.configure("Accent.TButton", font=("Segoe UI", 9, "bold"))

main_frame = ttk.Frame(root, padding=20)
main_frame.pack(fill="both", expand=True)

lbl_title = ttk.Label(main_frame, text="Battery Threshold", font=("Segoe UI", 14, "bold"))
lbl_title.pack(anchor="w")

lbl_subtitle = ttk.Label(main_frame, text="Atur batas pengisian daya untuk memperpanjang usia baterai.", font=("Segoe UI", 9), foreground="#666666")
lbl_subtitle.pack(anchor="w", pady=(0, 15))

frame_scales = ttk.LabelFrame(main_frame, text=" Batas Pengisian Baterai ", padding=15)
frame_scales.pack(fill="x", pady=(0, 15))

header_max_frame = ttk.Frame(frame_scales)
header_max_frame.pack(fill="x")
ttk.Label(header_max_frame, text="Batas Atas (Stop Charge):", font=("Segoe UI", 9)).pack(side="left")
lbl_max_val = ttk.Label(header_max_frame, text="95%", font=("Segoe UI", 9, "bold"))
lbl_max_val.pack(side="right")

scale_max = ttk.Scale(frame_scales, from_=50, to=100, command=on_slider_change)
scale_max.set(95)
scale_max.pack(fill="x", pady=(5, 15))

header_min_frame = ttk.Frame(frame_scales)
header_min_frame.pack(fill="x")
ttk.Label(header_min_frame, text="Batas Bawah (Start Charge):", font=("Segoe UI", 9)).pack(side="left")
lbl_min_val = ttk.Label(header_min_frame, text="90%", font=("Segoe UI", 9, "bold"))
lbl_min_val.pack(side="right")

scale_min = ttk.Scale(frame_scales, from_=40, to=95, command=on_slider_change)
scale_min.set(90)
scale_min.pack(fill="x", pady=(5, 5))

frame_buttons = ttk.Frame(main_frame)
frame_buttons.pack(fill="x", pady=(0, 15))

btn_on = ttk.Button(frame_buttons, text="Nyalakan (ON)", style="Accent.TButton", command=set_threshold_on)
btn_on.pack(side="left", expand=True, fill="x", padx=(0, 5))

btn_off = ttk.Button(frame_buttons, text="Matikan (OFF)", command=set_threshold_off)
btn_off.pack(side="left", expand=True, fill="x", padx=(5, 0))

frame_status = ttk.LabelFrame(main_frame, text=" Status Sistem ", padding=10)
frame_status.pack(fill="x")

lbl_status_val = ttk.Label(
    frame_status, 
    text="Klik 'Cek Status' untuk memperbarui...", 
    font=("Segoe UI", 9), 
    wraplength=330,
    foreground="#555555"
)
lbl_status_val.pack(fill="x", pady=(0, 5))

btn_status = ttk.Button(frame_status, text="Cek Status Saat Ini", command=check_status)
btn_status.pack(anchor="e")

root.after(500, check_status)

root.mainloop()