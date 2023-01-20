import seaborn as sns
import pandas as pd
import numpy as np
import matplotlib as mpl
import matplotlib.pyplot as plt

list = pd.read_csv("./resultData/School_3F_result_2023-01-19-22-50-03.csv").values.tolist()

plt.figure(figsize=(15, 8), dpi=130)
plt.axis("off")
sns.heatmap(list, square=True, cmap='hot', vmin=0, vmax=2000)

# plt.show()
plt.savefig("./result_heatmaps/School_3F_result_2023-01-19-22-50-03.png")