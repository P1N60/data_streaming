"""Plot results for Opgave 8: sorted estimates and medians for t=4,8,12, plus runtime."""
import csv
import matplotlib.pyplot as plt
import matplotlib.gridspec as gridspec
import numpy as np

T_VALUES = [4, 8, 12]

def read_csv(path):
    with open(path) as f:
        return list(csv.DictReader(f))

# --- Collect data ---
sorted_data  = {t: read_csv(f"opgave8_sorted_t{t}.csv")  for t in T_VALUES}
median_data  = {t: read_csv(f"opgave8_medians_t{t}.csv") for t in T_VALUES}
runtime_rows = read_csv("opgave8_runtime.csv")

s_val = int(sorted_data[T_VALUES[0]][0]["S"])

# Shared y limits from widest spread (t=4)
all_xs = [int(r["X"]) for r in sorted_data[4]]
y_min = min(all_xs) * 0.97
y_max = max(all_xs) * 1.03

# --- Figure 1: sorted estimates (top) + sorted medians (bottom) ---
fig, axes = plt.subplots(2, 3, figsize=(16, 9), sharey=True)
fig.suptitle("Opgave 8 — Count-Sketch for forskellige m", fontsize=13)

for col, t in enumerate(T_VALUES):
    m = 1 << t

    # Top: 100 sorted estimates
    rows = sorted_data[t]
    ranks = [int(r["rank"]) for r in rows]
    xs    = [int(r["X"])    for r in rows]

    ax = axes[0][col]
    ax.scatter(ranks, xs, s=12, color="steelblue")
    ax.axhline(s_val, color="red", linewidth=1.4, linestyle="--", label=f"S={s_val:,}")
    ax.set_title(f"m = 2^{t} = {m}")
    ax.set_xlabel("Rang $i$")
    if col == 0:
        ax.set_ylabel("Estimat $X_{(i)}$")
    ax.set_ylim(y_min, y_max)
    ax.ticklabel_format(axis="y", style="sci", scilimits=(0, 0))
    ax.legend(fontsize=7)

    # Bottom: 9 sorted medians
    mrows  = median_data[t]
    mranks = [int(r["rank"]) for r in mrows]
    meds   = [int(r["M"])    for r in mrows]

    ax2 = axes[1][col]
    ax2.scatter(mranks, meds, s=50, color="darkorange", zorder=5)
    ax2.axhline(s_val, color="red", linewidth=1.4, linestyle="--")
    ax2.set_xlabel("Rang $i$")
    if col == 0:
        ax2.set_ylabel("Median $M_{(i)}$")
    ax2.set_ylim(y_min, y_max)
    ax2.set_xticks(mranks)
    ax2.ticklabel_format(axis="y", style="sci", scilimits=(0, 0))

axes[0][1].set_title(f"100 sorterede estimater\nm = 2^8 = 256", fontsize=10)
fig.tight_layout()
fig.savefig("opgave8_estimates.png", dpi=150)
print("Gemt: opgave8_estimates.png")

# --- Figure 2: runtime bar chart ---
labels   = [r["metode"]     for r in runtime_rows]
ms_total = [int(r["ms_total"]) for r in runtime_rows]
ms_each  = [int(r["ms_per_exp"]) for r in runtime_rows]

x = np.arange(len(labels))
fig2, ax3 = plt.subplots(figsize=(9, 5))
bars = ax3.bar(x, ms_each, color=["#e05252"] + ["steelblue"] * (len(labels) - 1))
ax3.set_xticks(x)
ax3.set_xticklabels(labels, rotation=15, ha="right")
ax3.set_ylabel("Tid per eksperiment (ms)")
ax3.set_title("Køretidssammenligning: hashing m. chaining vs Count-Sketch")
for bar, val in zip(bars, ms_each):
    ax3.text(bar.get_x() + bar.get_width() / 2, bar.get_height() + 5,
             f"{val} ms", ha="center", va="bottom", fontsize=9)
fig2.tight_layout()
fig2.savefig("opgave8_runtime.png", dpi=150)
print("Gemt: opgave8_runtime.png")

plt.show()
