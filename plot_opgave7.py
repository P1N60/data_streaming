"""Plot results for Opgave 7 from opgave7_sorted.csv and opgave7_medians.csv."""
import csv
import matplotlib.pyplot as plt

def read_csv(path):
    with open(path) as f:
        reader = csv.DictReader(f)
        rows = list(reader)
    return rows

# --- Plot 1: 100 sorterede estimater ---
rows = read_csv("opgave7_sorted.csv")
ranks  = [int(r["rank"]) for r in rows]
xs     = [int(r["X"])    for r in rows]
s_val  = int(rows[0]["S"])

# Fælles y-akse: brug spændet fra de 100 estimater på begge plots
y_min = min(xs) * 0.98
y_max = max(xs) * 1.02

fig, axes = plt.subplots(1, 2, figsize=(13, 5), sharey=True)

ax = axes[0]
ax.scatter(ranks, xs, s=18, color="steelblue", label="$X_{(i)}$")
ax.axhline(s_val, color="red", linewidth=1.5, linestyle="--", label=f"S = {s_val:,}")
ax.set_xlabel("Rang $i$")
ax.set_ylabel("Estimat")
ax.set_title("100 sorterede Count-Sketch estimater")
ax.set_ylim(y_min, y_max)
ax.legend()
ax.ticklabel_format(axis="y", style="sci", scilimits=(0, 0))

# --- Plot 2: 9 sorterede medianer ---
mrows   = read_csv("opgave7_medians.csv")
mranks  = [int(r["rank"]) for r in mrows]
medians = [int(r["M"])    for r in mrows]

ax2 = axes[1]
ax2.scatter(mranks, medians, s=60, color="darkorange", zorder=5, label="$M_{(i)}$")
ax2.axhline(s_val, color="red", linewidth=1.5, linestyle="--", label=f"S = {s_val:,}")
ax2.set_xlabel("Rang $i$")
ax2.set_title("9 sorterede medianer (median-of-groups)")
ax2.set_xticks(mranks)
ax2.set_ylim(y_min, y_max)
ax2.legend()
ax2.ticklabel_format(axis="y", style="sci", scilimits=(0, 0))

plt.tight_layout()
plt.savefig("opgave7.png", dpi=150)
print("Plot gemt som 'opgave7.png'")
plt.show()
