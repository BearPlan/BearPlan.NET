#!/bin/bash
# 一键拉取主仓库及全部 submodule（BearPlan.Core / BearPlan.Admin）最新代码
#
# 用法：
#   chmod +x pull.sh
#   ./pull.sh              # 拉取主仓库 + 所有 submodule
#   ./pull.sh --init       # 全新克隆后初始化 submodule（等价于 git submodule update --init --recursive）
#   ./pull.sh --no-main    # 只更新 submodule，不拉取主仓库

set -e

WORK_DIR=$(cd "$(dirname "$0")" && pwd)
cd "$WORK_DIR"

# 颜色输出
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
CYAN='\033[0;36m'
NC='\033[0m'

info()  { echo -e "${CYAN}[INFO]${NC}  $1"; }
ok()    { echo -e "${GREEN}[OK]${NC}    $1"; }
warn()  { echo -e "${YELLOW}[WARN]${NC}  $1"; }

# 参数解析
DO_INIT=0
SKIP_MAIN=0
for arg in "$@"; do
  case "$arg" in
    --init)    DO_INIT=1 ;;
    --no-main) SKIP_MAIN=1 ;;
    *) echo "未知参数：$arg"; echo "用法：$0 [--init|--no-main]"; exit 1 ;;
  esac
done

# 检查是否在 git 仓库内
if [[ ! -d .git ]]; then
  echo "❌ 当前目录不是 git 仓库：$WORK_DIR"
  exit 1
fi

# --init：全新克隆后的初始化场景，直接递归拉取所有 submodule
if [[ $DO_INIT -eq 1 ]]; then
  info "初始化所有 submodule（含嵌套）..."
  git submodule update --init --recursive
  ok "submodule 初始化完成"
  exit 0
fi

# 1. 拉取主仓库
if [[ $SKIP_MAIN -eq 0 ]]; then
  info "拉取主仓库 $(basename "$WORK_DIR") ..."
  # 暂存本地未提交改动，避免 pull 冲突
  if [[ -n "$(git status --porcelain)" ]]; then
    warn "主仓库有未提交改动，已自动 stash"
    git stash push -u -m "auto-stash before pull at $(date '+%F %T')"
  fi
  git pull --ff-only
  ok "主仓库已更新"
fi

# 2. 同步 submodule 配置（.gitmodules 变化时 url/path 可能改过）
info "同步 .gitmodules 配置..."
git submodule sync --recursive

# 3. 更新所有 submodule 到主仓库记录的 commit
info "更新 submodule（BearPlan.Core / BearPlan.Admin）..."
git submodule update --init --recursive --merge

ok "全部拉取完成"
echo
info "当前 submodule 指针："
git submodule status
